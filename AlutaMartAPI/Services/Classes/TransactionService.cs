using AlutaMartAPI.Database;
using AlutaMartAPI.Models;
using AlutaMartAPI.SQLQueries;
using AlutaMartAPI.Utilities;
using Microsoft.EntityFrameworkCore;
using Npgsql;
namespace AlutaMartAPI.Services;

public class TransactionService(
        IUnitOfWork _unitOfWork, 
        IResponseService _responseService, 
        IPaystackService _paystackService, 
        INotificationService _notificationService
    ) : BaseDBService(_unitOfWork, _responseService), ITransactionService
{
    public async Task<ServiceResponse<string>> VerifyPaymentInflowAsync(Guid paymentId)
    {
        var payment = await _unitOfWork.Context.PaymentInflows
            .AsNoTracking()
            .Where(x => x.Id == paymentId)
            .Select(x => new
            {
                x.Amount,
                x.Reference,
                x.ExternalReference,
                x.Narration,
                x.Status,
                x.Charges,
                x.Revenue,
                x.CurrencyId,
                x.Processor,
                CurrencyCode = x.Currency.Code,
                BuyerProfileId = x.ProfileId,
                BuyerFirstName = x.Profile.FirstName,
                BuyerLastName = x.Profile.LastName,
                x.AdId,
                AdTitle = x.Ads.Title,
                x.VendorId,
                VendorEmail = x.Vendor.Profile.Email,
                VendorFirstName = x.Vendor.Profile.FirstName,
                VendorProfileId = x.Vendor.ProfileId
            })
            .FirstOrDefaultAsync();

        if(payment is null || payment.VendorId == Guid.Empty) return _responseService.ErrorResponse<string>("Payment not found");

        if(payment.Status == PaymentStatus.Successful) return _responseService.SuccessResponse(payment.AdId.ToString(), "payment verified successfully...");

        if(payment.Status != PaymentStatus.Initiated) return _responseService.ErrorResponse<string>("Payment verification was not successful");

        if(payment.Processor == PaymentProcessor.Paystack)
        {
            var verificationResponse = await _paystackService.VerifyPaymentAsync(payment.ExternalReference);
            if(!verificationResponse.status) return _responseService.ErrorResponse<string>("Payment not verified");

            var processorDataLog = new ProcessorDataLog
            {
                ProcessorData = verificationResponse.ToJson(),
                PaymentInflowId = paymentId,
                Processor = PaymentProcessor.Paystack,
            };
            await _unitOfWork.Context.AddAsync(processorDataLog);

            if(verificationResponse.data.status != "success" || verificationResponse.data.amount != (payment.Amount *100))
            {
                processorDataLog.Status = PaymentStatus.Failed;
                await _unitOfWork.CommitAsync();

                await _unitOfWork.Context.Database.ExecuteSqlRawAsync(TransactionSQL.SetPaymentInflowAsFailed, new NpgsqlParameter("@id", paymentId));
                return _responseService.ErrorResponse<string>("payment vefification failed");
            }
            processorDataLog.Status = PaymentStatus.Successful;
        }
        else
        {
            return _responseService.ErrorResponse<string>("Invalid payment currency");
        }

        await _unitOfWork.Context.Database.ExecuteSqlRawAsync(TransactionSQL.SetPaymentInflowAsSuccessful, new NpgsqlParameter("@id", paymentId));
        
        var transaction = new Transaction
        {
            Amount = payment.Amount,
            Charges = payment.Charges,
            Revenue = payment.Revenue,
            Reference = payment.Reference,
            CurrencyId = payment.CurrencyId,
            PaymentInflowId = paymentId,
            ProfileId = payment.VendorProfileId,
            Narration = payment.Narration,
            PaymentType = PaymentType.Inflow,
            Status = PaymentStatus.Successful
        };

        await _unitOfWork.Context.AddAsync(transaction);

        var regBuyer = new PurchasedAd
        {
            ProfileId = payment.BuyerProfileId,
            AdId = payment.AdId.Value,
            VendorId = payment.VendorId.Value,
            PaymentInflowId = paymentId
        };

        var existingBuyer = await _unitOfWork.Context.Buyers
            .AsNoTracking()
            .Where(x => x.ProfileId == payment.BuyerProfileId)
            .Select(x => x.Id)
            .FirstOrDefaultAsync();
        
        if(existingBuyer != Guid.Empty) regBuyer.BuyerId = existingBuyer;

        await _unitOfWork.Context.AddAsync(regBuyer);

        var walletId = await _unitOfWork.Context.Wallets
            .AsNoTracking()
            .Where(x => x.CurrencyId == payment.CurrencyId && x.ProfileId == payment.VendorProfileId)
            .Select(x => x.Id)
            .FirstOrDefaultAsync();
        
        if(walletId == Guid.Empty)
        {
            var wallet = new Wallet
            {
                ProfileId = payment.VendorProfileId,
                CurrencyId = payment.CurrencyId,
                Amount = payment.Revenue
            };
            await _unitOfWork.Context.AddAsync(wallet);
        }
        else
        {
            var parameters = new List<object>
            {
                new NpgsqlParameter("@amountPaid", payment.Revenue),
                new NpgsqlParameter("@id", walletId)
            };
            await _unitOfWork.Context.Database.ExecuteSqlRawAsync(TransactionSQL.UpdateWalletBalance, parameters);
        }
        
        await _unitOfWork.CommitAsync();

        await CreditAdminAsync(transaction.Charges, payment.CurrencyId, payment.AdTitle, payment.Reference, payment.Processor);

        await CourseEngagementAsync(payment.CourseId.Value, payment.LearnerProfileId, true);

        await _notificationService.CourseEnrollmentNoticeEmailAsync(payment.ExpertEmail, payment.ExpertFirstName, 
            $"{payment.LearnerFirstName} {payment.LearnerLastName}", payment.CourseTitle);

        return _responseService.SuccessResponse(payment.CourseId.ToString(), "course enrolled successfully...");
    }

    private async Task CreditAdminAsync(double amount, Guid currencyId, string courseTitle, double reference, PaymentProcessor paymentProcessor)
    {
        var adminProfileId = await _unitOfWork.Context.Profiles
            .AsNoTracking()
            .Where(x => x.Email == Constants.AdminEmail)
            .Select(x => x.Id)
            .FirstOrDefaultAsync();

        if(adminProfileId == Guid.Empty) return;

        var adminPaymentInflow = new PaymentInflow
        {
            Amount = amount,
            Reference = AppUtilities.RandomLong(12, false, true),
            Charges = 0,
            Revenue = amount,
            Status = PaymentStatus.Successful,
            CurrencyId = currencyId,
            ProfileId = adminProfileId,
            Processor = paymentProcessor,
            Narration = $"Revenue from payment on {courseTitle} with Ref: {reference}"
        };

        await _unitOfWork.Context.AddAsync(adminPaymentInflow);

        var transaction = new Transaction
        {
            Amount = amount,
            Charges = 0,
            Revenue = amount,
            Reference = adminPaymentInflow.Reference,
            CurrencyId = currencyId,
            PaymentInflowId = adminPaymentInflow.Id,
            ProfileId = adminProfileId,
            Narration = adminPaymentInflow.Narration,
            PaymentType = PaymentType.Inflow,
            Status = PaymentStatus.Successful
        };

        await _unitOfWork.Context.AddAsync(transaction);

        var walletId = await _unitOfWork.Context.Wallets
            .AsNoTracking()
            .Where(x => x.CurrencyId == currencyId && x.ProfileId == adminProfileId)
            .Select(x => x.Id)
            .FirstOrDefaultAsync();
        
        if(walletId == Guid.Empty)
        {
            var wallet = new Wallet
            {
                ProfileId = adminProfileId,
                CurrencyId = currencyId,
                Amount = amount
            };
            await _unitOfWork.Context.AddAsync(wallet);
        }
        else
        {
            var parameters = new List<object>
            {
                new NpgsqlParameter("@amountPaid", amount),
                new NpgsqlParameter("@id", walletId)
            };
            await _unitOfWork.Context.Database.ExecuteSqlRawAsync(TransactionSQL.UpdateWalletBalance, parameters);
        }
        
        await _unitOfWork.CommitAsync();
    }
}   
using AlutaMartAPI.Database;
using AlutaMartAPI.DTOs;
using AlutaMartAPI.ModelObjects;
using AlutaMartAPI.Utilities;
using Microsoft.EntityFrameworkCore;


namespace AlutaMartAPI.Services;
public class WalletServices(IUnitOfWork _unitOfWork, IResponseService _responseService) : IWalletService
{
    public async Task<ServiceResponse<GetWalletDTO>> GetWalletAsync(UserDTO user)
    {
        var walletData = await _unitOfWork.Context.Wallets
            .AsNoTracking()
            .Where(x => x.ProfileId == user.Id)
            .Select(x => new 
            {
                x.Amount, 
                TotalSales = _unitOfWork.Context.PurchasedAds
                    .Count(x => x.VendorId == user.VendorId)
            })
            .FirstOrDefaultAsync(); 

        if (walletData == null) return _responseService.ErrorResponse<GetWalletDTO>("Wallet not found.");

        // var transactions = await _unitOfWork.Context.Transactions
        //     .AsNoTracking() 
        //     .Where(x => x.ProfileId == user.Id) 
        //     .Select(x => new GetWalletTransactionDTO
        //     {
        //         // If the transaction is a payment inflow, get the course title, otherwise label it as "Withdrawal"
        //         Item = x.PaymentInflow != null ? x.PaymentInflow.Ads.Title : "Withdrawal", 

        //         InitiatorName = x.PaymentInflow != null 
        //             ? $"{ x.PaymentInflow.Profile.FirstName }  { x.PaymentInflow.Profile.LastName }" 
        //             : $"{ x.PaymentOutflow.Profile.FirstName } { x.PaymentOutflow.Profile.LastName }",

        //         Amount = x.Amount,
        //         PaymentType = x.PaymentType,
        //         DatePaid = x.PaymentInflow != null ? x.PaymentInflow.DatePaid : x.PaymentOutflow.Created
        //     })
        //     .ToListAsync();

        var wallet = new GetWalletDTO
            {
                Balance = walletData.Amount,
                TotalSales = walletData.TotalSales,
                // TransactionHistory = transactions
            };

        return _responseService.SuccessResponse(wallet, "Wallet");
    }
}

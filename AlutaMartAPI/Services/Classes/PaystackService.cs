using AlutaMartAPI.DTOs;
using AlutaMartAPI.Utilities;
using Paystack.Net.SDK.Models;
using Paystack.Net.SDK.Transactions;

namespace AlutaMartAPI.Services;

public class PaystackService(IResponseService responseService, ILoggerFactory logger) : IPaystackService
{
    private readonly IResponseService _responseService = responseService;
    protected readonly ILogger _logger = logger.CreateLogger("Paystack-Service");

    public async Task<ServiceResponse<PaymentInitalizationResponseModel>> CreatePaymentLinkAsync(double amount, string currencyCode, UserDTO user, Guid paymentId, double reference)
    {
        currencyCode = currencyCode.TrimAllSpace().ToLower();

        if(currencyCode != "ngn") return _responseService.ErrorResponse<PaymentInitalizationResponseModel>("can't process this currency on paystack");
        
        var callbackUrl = $"{Constants.FrontendBaseUrl}payment/verification/{paymentId}";
        
        var transaction = new PaystackTransaction(Constants.PaystackSecreteKey);

        var response = await transaction.InitializeTransaction(user.Email, Convert.ToInt32(amount*100), user.FirstName, user.LastName, callbackUrl, reference.ToString(), true);
        _logger.LogInformation("response from paystack: {data}", response.ToJson());
        return _responseService.SuccessResponse(response);
    }

    public async Task<TransactionResponseModel> VerifyPaymentAsync(string reference)
    {
        var transaction = new PaystackTransaction(Constants.PaystackSecreteKey);
       var response = await transaction.VerifyTransaction(reference);
       _logger.LogInformation("response from paystack: {data}", response.ToJson());
       return response;
    }
}
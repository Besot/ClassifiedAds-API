using AlutaMartAPI.DTOs;
using AlutaMartAPI.Utilities;
using Paystack.Net.SDK.Models;

namespace AlutaMartAPI.Services;

public interface IPaystackService
{
    Task<TransactionResponseModel> VerifyPaymentAsync(string reference);
    Task<ServiceResponse<PaymentInitalizationResponseModel>> CreatePaymentLinkAsync(double amount, string currencyCode, UserDTO user, Guid paymentId, double reference);
}
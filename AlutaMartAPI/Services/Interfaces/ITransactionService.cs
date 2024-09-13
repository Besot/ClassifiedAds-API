using AlutaMartAPI.Utilities;

namespace AlutaMartAPI.Services;

public interface ITransactionService
{
    Task<ServiceResponse<string>> VerifyPaymentInflowAsync(Guid paymentId);
}
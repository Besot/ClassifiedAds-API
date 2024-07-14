
using AlutaMartAPI.ModelObjects;
using AlutaMartAPI.Utilities;

namespace AlutaMartAPI.Services;
public interface ICurrencyService
{
    Task<ServiceResponse<List<GetCurrencyDTO>>> GetAsync();
}
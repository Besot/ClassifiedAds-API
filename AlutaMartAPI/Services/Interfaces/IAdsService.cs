using AlutaMartAPI.DTOs;
using AlutaMartAPI.Utilities;

namespace AlutaMartAPI.Services;
public interface IAdsService
{
    Task<ServiceResponse<string>> CreateAdsAsync(CreateAdsDTO model, UserDTO user);
    Task<ServiceResponse<PagedList<GetAdsDTO>>> GetAsync(int Page = 1, int pageSize = 15);
}
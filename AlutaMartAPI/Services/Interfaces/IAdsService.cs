using AlutaMartAPI.DTOs;
using AlutaMartAPI.Utilities;

namespace AlutaMartAPI.Services;
public interface IAdsService
{
    Task<ServiceResponse<string>> CreateAdsAsync(CreateAdsDTO model, UserDTO user);
    Task<ServiceResponse<PagedList<GetAdsDTO>>> GetAsync(int Page = 1, int pageSize = 15);
    Task<ServiceResponse<PagedList<GetAdsDTO>>> GetByVendorIdAsync(Guid vendorId, int page = 1, int pageSize = 10);
    Task<ServiceResponse<GetAdDetailsDTO>> GetDetailsAsync(Guid adId, UserDTO user);
    Task<ServiceResponse<PagedList<GetAdsDTO>>> SearchAsync(string searchQuery, Guid? adsCategoryId, int page, int pageSize, bool? isFree);
    Task<ServiceResponse<string>> UpdateAdAsync(Guid adId, CreateAdsDTO model, UserDTO user);
    Task<ServiceResponse<string>> DeleteAdAsync(Guid adId, Guid vendorId, bool isAdmin);
    Task<ServiceResponse<int>> SetIsFeaturedFalseForExpiredAdsAsync(int batchSize);
    Task<ServiceResponse<string>> PurchaseAsync(UserDTO user, Guid adId);


}
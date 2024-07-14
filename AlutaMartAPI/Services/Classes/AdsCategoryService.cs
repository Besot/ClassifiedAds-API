using AlutaMartAPI.Database;
using AlutaMartAPI.DTOs;
using AlutaMartAPI.Utilities;
using Microsoft.EntityFrameworkCore;
namespace AlutaMartAPI.Services;
public class AdsCategoryService(IUnitOfWork unitOfWork, IResponseService responseService, IMemoryCacheService _memoryCache) 
: BaseDBService(unitOfWork, responseService), IAdsCategoryService
{
    public async Task<ServiceResponse<List<GetAdsCategoryDTO>>> GetAsync()
    {
        var cacheResponse = _memoryCache.GetFromLocalCache<List<GetAdsCategoryDTO>>(Constants.AdsCategoryCacheKey);
        if(!cacheResponse.Status)
        {
            cacheResponse.Data = await _unitOfWork.Context.AdsCategories
                .AsNoTracking()
                .Select(x => new GetAdsCategoryDTO
                { 
                    Id = x.Id, 
                    Name = x.Name,
                    Brand = x.Brand 
                })
                .ToListAsync();
            _memoryCache.SaveToLocalCache(Constants.AdsCategoryCacheKey, cacheResponse.Data);     
        }
            
        return _responseService.SuccessResponse(cacheResponse.Data);
    }
}
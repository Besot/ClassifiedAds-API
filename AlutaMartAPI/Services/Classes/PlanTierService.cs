using AlutaMartAPI.Database;
using AlutaMartAPI.DTOs;
using AlutaMartAPI.Utilities;
using Microsoft.EntityFrameworkCore;

namespace AlutaMartAPI.Services;
public class PlanTierService(IUnitOfWork _unitOfWork, IResponseService _responseService, IMemoryCacheService _memoryCache) : IPlanTierService
{
    public async Task<ServiceResponse<List<GetPlanTierDTO>>> GetAsync()
    {
        
        var cacheResponse = _memoryCache.GetFromLocalCache<List<GetPlanTierDTO>>(Constants.PlanTierCacheKey);
        if(!cacheResponse.Status)
        {
            cacheResponse.Data = await _unitOfWork.Context.PlanTiers
                .AsNoTracking()
                .Select(x => new GetPlanTierDTO
                { 
                    Id = x.Id, 
                    Name = x.Name,
                    Amount = x.Amount,
                    MaxAds = x.MaxAds,
                    MaxPicture = x.MaxPicture,
                    MaxFeatured = x.MaxPicture
                })
                .ToListAsync();
            _memoryCache.SaveToLocalCache(Constants.PlanTierCacheKey, cacheResponse.Data);     
        }
            
        return _responseService.SuccessResponse(cacheResponse.Data);
    }
}
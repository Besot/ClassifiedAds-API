using AlutaMartAPI.Database;
using AlutaMartAPI.DTOs;
using AlutaMartAPI.Utilities;
using Microsoft.EntityFrameworkCore;

namespace AlutaMartAPI.Services;
public class InstitutionService(IUnitOfWork _unitOfWork, IResponseService _responseService, IMemoryCacheService _memoryCache) : IInstitutionService
{
    public async Task<ServiceResponse<List<GetInstitutionDTO>>> GetAsync()
    {
        
        var cacheResponse = _memoryCache.GetFromLocalCache<List<GetInstitutionDTO>>(Constants.InstitutionCacheKey);
        if(!cacheResponse.Status)
        {
            cacheResponse.Data = await _unitOfWork.Context.Institutions
                .AsNoTracking()
                .Select(x => new GetInstitutionDTO
                { 
                    Id = x.Id, 
                    Name = x.Name,
                    Abbrev = x.Abbrev,
                    State = x.State
                })
                .ToListAsync();
            _memoryCache.SaveToLocalCache(Constants.InstitutionCacheKey, cacheResponse.Data);     
        }
            
        return _responseService.SuccessResponse(cacheResponse.Data);
    }
}
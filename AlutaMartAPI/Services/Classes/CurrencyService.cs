using AlutaMartAPI.Database;
using AlutaMartAPI.ModelObjects;
using AlutaMartAPI.Utilities;
using Microsoft.EntityFrameworkCore;

namespace AlutaMartAPI.Services;
    public class CurrencyService(IUnitOfWork _unitOfWork, IResponseService _responseService, IMemoryCacheService _memoryCache) : ICurrencyService
    {
        public async Task<ServiceResponse<List<GetCurrencyDTO>>> GetAsync()
        {
            var cacheResponse = _memoryCache.GetFromLocalCache<List<GetCurrencyDTO>>(Constants.CurrencyCacheKey);
            if(!cacheResponse.Status)
            {
                cacheResponse.Data = await _unitOfWork.Context.Currencies
                    .AsNoTracking()
                    .Select(x => new GetCurrencyDTO
                    { 
                        Id = x.Id, 
                        Code = x.Code, 
                        Name = x.Name 
                    })
                    .ToListAsync();
                _memoryCache.SaveToLocalCache(Constants.CurrencyCacheKey, cacheResponse.Data);     
            }
                
            return _responseService.SuccessResponse(cacheResponse.Data);
        }
    }
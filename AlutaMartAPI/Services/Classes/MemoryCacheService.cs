using Microsoft.Extensions.Caching.Memory;
using AlutaMartAPI.Utilities;

namespace AlutaMartAPI.Services;

public class MemoryCacheService(IMemoryCache _memoryCache, IResponseService _responseService) : IMemoryCacheService
{
    private readonly MemoryCacheEntryOptions cacheOption = new() { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30) };
    public void SaveToLocalCache(string key, object data) => _memoryCache.Set(key, data, cacheOption);

    public ServiceResponse<T> GetFromLocalCache<T>(string key)
    {
        if(!_memoryCache.TryGetValue(key, out T data)) return _responseService.ErrorResponse<T>($"data not found for {key}");
        return _responseService.SuccessResponse(data);
    }
}
using AlutaMartAPI.Utilities;

namespace AlutaMartAPI.Services;

public interface IMemoryCacheService
{
    void SaveToLocalCache(string key, object data);
    ServiceResponse<T> GetFromLocalCache<T>(string key);
}
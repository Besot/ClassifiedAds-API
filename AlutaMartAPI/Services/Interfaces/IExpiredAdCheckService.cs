using AlutaMartAPI.Utilities;

namespace AlutaMartAPI.Services;
    public interface IExpiredAdCheckService
    {
        Task<ServiceResponse<int>> SetIsFeaturedFalseForExpiredAdsAsync(int batchSize);
    }
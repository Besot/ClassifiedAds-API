using AlutaMartAPI.DTOs;
using AlutaMartAPI.Utilities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AlutaMartAPI.Services
{
    public interface IRatingService
    {
        Task<ServiceResponse<string>> CreateRatingAsync(CreateRatingDTO model, UserDTO currentUser);
        Task<ServiceResponse<PagedList<GetRatingDTO>>> GetRatingsForVendorAsync(Guid vendorId, int page = 1, int pageSize = 10);
        Task<ServiceResponse<PagedList<GetRatingDTO>>> GetRatingsForAdAsync(Guid adId, int page = 1, int pageSize = 10);
        Task<ServiceResponse<double>> GetAverageRatingForVendorAsync(Guid vendorId);
        Task<ServiceResponse<double>> GetAverageRatingForAdAsync(Guid adId);
    }
}

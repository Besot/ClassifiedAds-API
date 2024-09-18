using AlutaMartAPI.DTOs;
using AlutaMartAPI.Utilities;

namespace AlutaMartAPI.Services;
    public interface IAdminService
    {
        Task<ServiceResponse<string>> CreateAdminAsync(CreateAdminDTO model);
        Task<ServiceResponse<string>> SetAdminProfileStateAsync(Guid profileId, bool isActive);
    }
using AlutaMartAPI.DTOs;
using AlutaMartAPI.ModelObjects;
using AlutaMartAPI.Utilities;

namespace AlutaMartAPI.Services;
    public interface IAdminService
    {
        Task<ServiceResponse<string>> CreateAdminAsync(CreateAdminDTO model);
        Task<ServiceResponse<string>> SetAdminProfileStateAsync(Guid profileId, bool isActive);
        Task<ServiceResponse<PagedList<GetAdminDTO>>> GetAsync(int page = 1, int pageSize = 15);
        Task<ServiceResponse<GetAdminDTO>> GetByAdminIdAsync(Guid profileId);

    }
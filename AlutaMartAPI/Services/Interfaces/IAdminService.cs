using AlutaMartAPI.DTOs;
using AlutaMartAPI.Utilities;

namespace AlutaMartAPI.Services;
    public interface IAdminService
    {
        Task<ServiceResponse<string>> CreateAdminAsync(CreateAdminDTO model);
    }
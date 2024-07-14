using AlutaMartAPI.DTOs;
using AlutaMartAPI.Utilities;

namespace AlutaMartAPI.Services;
public interface IVendorService
{
    Task<ServiceResponse<string>> CreateAsync(CreateVendorDTO model, UserDTO user);
    Task<ServiceResponse<string>> DeleteVendorAsync(Guid profileId);
    Task<ServiceResponse<PagedList<GetVendorDTO>>> GetAsync(int page = 1, int pageSize = 15);
    Task<ServiceResponse<GetVendorDTO>> GetDetailsAsync(Guid vendorId);
}
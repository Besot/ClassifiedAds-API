using AlutaMartAPI.DTOs;
using AlutaMartAPI.Utilities;

namespace AlutaMartAPI.Services;
public interface IVendorService
{
    Task<ServiceResponse<string>> CreateAsync(CreateVendorDTO model, UserDTO user);
}
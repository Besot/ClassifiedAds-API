using AlutaMartAPI.DTOs;
using AlutaMartAPI.ModelObjects;
using AlutaMartAPI.Utilities;

namespace AlutaMartAPI.Services;
public interface IWalletService
{
    Task<ServiceResponse<GetWalletDTO>> GetWalletAsync(UserDTO user);
}
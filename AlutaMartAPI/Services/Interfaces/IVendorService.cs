using AlutaMartAPI.DTOs;
using AlutaMartAPI.Utilities;

namespace AlutaMartAPI.Services;
public interface IVendorService
{
    Task<ServiceResponse<string>> AddBankAccountAsync(CreateBankAccountDTO createBankAccountDTO, UserDTO currentUser);
    Task<ServiceResponse<string>> AddSecurityQuestionAsync(AddSecurityQuestionDTO addSecurityQuestionDTO, UserDTO currentUser);
    Task<ServiceResponse<string>> CreateAsync(CreateVendorDTO model, UserDTO user);
    Task<ServiceResponse<string>> CreateTransactionPINAsync(CreateTransactionPINDTO createTransactionPINDTO, UserDTO currentUser);
    Task<ServiceResponse<string>> DeleteVendorAsync(Guid profileId);
    Task<ServiceResponse<PagedList<GetVendorDTO>>> GetAsync(int page = 1, int pageSize = 15);
    Task<ServiceResponse<GetBankAccountsDTO>> GetBankAccounts(UserDTO currentUser);
    Task<ServiceResponse<GetVendorDTO>> GetDetailsAsync(Guid vendorId);
}
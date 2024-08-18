using AlutaMartAPI.DTOs;
using AlutaMartAPI.Utilities;

namespace AlutaMartAPI.Services;
    public interface IAuthService
    {
        Task<ServiceResponse<TokenResponseDTO>> EmailLoginAsync(EmailLoginDTO model);
        Task<ServiceResponse<string>> CreateAccountAsync(CreateUserDTO model, bool isLearner = false);
        Task<ServiceResponse<string>> ResetPasswordAsync(ResetPasswordDTO model);
        Task<ServiceResponse<string>> CreateNewPasswordAsync(CreatePasswordDTO model);
        Task<ServiceResponse<string>> SeedAdminAsync();
        Task<ServiceResponse<string>> ResendEmailVerificationAsync(UserDTO user, bool isLearner = false);
        Task<ServiceResponse<string>> VerifiyAccountAsync(int token);
        Task<ServiceResponse<string>> SetPasswordAsync(int token, SetPasswordDTO model);

    }

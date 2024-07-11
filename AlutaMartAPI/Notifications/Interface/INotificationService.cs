using AlutaMartAPI.Models;
using AlutaMartAPI.Utilities;

namespace AlutaMartAPI.Services;

public interface INotificationService
{
	Task ResetPasswordEmailAsync(string email, string firstName, string token);
	Task UpdatePasswordEmailAsync(string email, string firstName);
	Task UserEmailVerificationAsync(string email, string firstName, string token, bool isLearner = false);
    Task SetPasswordEmailAsync(string email, string firstName, Roles adminRole, string token);
	Task SetPasswordSuccessEmailAsync(string email, string firstName, Roles role);
}
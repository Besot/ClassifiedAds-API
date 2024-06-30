using AlutaMartAPI.Models;

namespace AlutaMartAPI.Services;

public interface INotificationService
{
	Task ResetPasswordEmailAsync(string email, string firstName, string token);
	Task UpdatePasswordEmailAsync(string email, string firstName);
	Task UserEmailVerificationAsync(string email, string firstName, string token, bool isLearner = false);
}
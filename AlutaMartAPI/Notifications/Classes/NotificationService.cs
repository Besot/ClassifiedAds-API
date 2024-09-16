using AlutaMartAPI.Utilities;

namespace AlutaMartAPI.Services;

public class NotificationService(IWebHostEnvironment webHostEnvironment, IMailSenderService mailSenderService) : INotificationService
{
	private readonly IWebHostEnvironment _webHostEnv = webHostEnvironment;
	private readonly IMailSenderService _mailSenderService = mailSenderService;

    public async Task ResetPasswordEmailAsync(string email, string firstName, string token)
	{
		var fullPath = Path.Combine(_webHostEnv.WebRootPath, "reset_password_email.html");
		var htmlMessage = File.ReadAllText(fullPath);
		htmlMessage = htmlMessage.Replace("{{firstName}}", firstName.ToTitleCase());
		htmlMessage = htmlMessage.Replace("{{code}}", token);

		await _mailSenderService.SendByPostMarkAppAsync(htmlMessage, email, "Reset Password Verification Code");
	}

	public async Task UpdatePasswordEmailAsync(string email, string firstName)
	{
		var fullPath = Path.Combine(_webHostEnv.WebRootPath, "password_update_notice_email.html");
		var htmlMessage = File.ReadAllText(fullPath);
		htmlMessage = htmlMessage.Replace("{{firstName}}", firstName.ToTitleCase());

		await _mailSenderService.SendByPostMarkAppAsync(htmlMessage, email, "Account Updated Successfully");
	}

	public async Task UserEmailVerificationAsync(string email, string firstName, string token, bool isBuyer = false)
	{
		var fullPath = Path.Combine(_webHostEnv.WebRootPath, "user_signup_email.html");
		var htmlMessage = File.ReadAllText(fullPath);
		htmlMessage = htmlMessage.Replace("{{firstName}}", firstName.ToTitleCase());
		htmlMessage = htmlMessage.Replace("{{code}}", token);
		htmlMessage = isBuyer ? htmlMessage.Replace("{{frontendBaseUrl}}", Constants.FrontendBaseUrl) 
			: htmlMessage.Replace("{{frontendBaseUrl}}", Constants.ExpertFrontendBaseUrl);

		await _mailSenderService.SendByPostMarkAppAsync(htmlMessage, email, "Email Verification");
	}

	public async Task SetPasswordEmailAsync(string email, string firstName, Roles adminRole, string token)
    {
        var fullPath = Path.Combine(_webHostEnv.WebRootPath, "set_password_email.html");
		var htmlMessage = File.ReadAllText(fullPath);
		htmlMessage = htmlMessage.Replace("{{firstName}}", firstName.ToTitleCase());
		htmlMessage = htmlMessage.Replace("{{role}}", adminRole.Name());
		htmlMessage = htmlMessage.Replace("{{code}}", token);
		htmlMessage = htmlMessage.Replace("{{frontendBaseUrl}}", Constants.AdminFrontendBaseUrl);


		await _mailSenderService.SendByPostMarkAppAsync(htmlMessage, email, $"{adminRole.Name()} Account Password");
	}

	public async Task SetPasswordSuccessEmailAsync(string email, string firstName, Roles role)
    {
        var fullPath = Path.Combine(_webHostEnv.WebRootPath, "set_password_success_email.html");
		var htmlMessage = File.ReadAllText(fullPath);
		htmlMessage = htmlMessage.Replace("{{firstName}}", firstName.ToTitleCase());
		htmlMessage = htmlMessage.Replace("{{role}}", role.Name());
		htmlMessage = htmlMessage.Replace("{{frontendBaseUrl}}", Constants.AdminFrontendBaseUrl);


		await _mailSenderService.SendByPostMarkAppAsync(htmlMessage, email, "Account Updated Successfully");
	}

	public async Task VendorOnboardingEmailAsync(string email, string firstName)
	{
		var fullPath = Path.Combine(_webHostEnv.WebRootPath, "vendor_onboarding_email.html");
		var htmlMessage = File.ReadAllText(fullPath);
		htmlMessage = htmlMessage.Replace("{{firstName}}", firstName.ToTitleCase());
		htmlMessage = htmlMessage.Replace("{{frontendBaseUrl}}", Constants.FrontendBaseUrl);

		await _mailSenderService.SendByPostMarkAppAsync(htmlMessage, email, "Welcome to AlutaMart as a Vendor!");
	}

	public async Task AdPurchaseNoticeEmailAsync(string email, string vendorFirstName, string buyerFullName, string AdTitle)
    {
		var fullPath = Path.Combine(_webHostEnv.WebRootPath, "ad_purchase_notice.html");
		var htmlMessage = File.ReadAllText(fullPath);
		htmlMessage = htmlMessage.Replace("{{vendorFirstName}}", vendorFirstName.ToTitleCase());
		htmlMessage = htmlMessage.Replace("{{buyerFullName}}", buyerFullName.ToTitleCase());
		htmlMessage = htmlMessage.Replace("{{adTitle}}", AdTitle.ToTitleCase());
		await _mailSenderService.SendByPostMarkAppAsync(htmlMessage, email,  "Ad Purchase Notice!");
    }
}
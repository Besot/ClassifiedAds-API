using AlutaMartAPI.Database;
using AlutaMartAPI.DTOs;
using AlutaMartAPI.Models;
using AlutaMartAPI.SQLQueries;
using AlutaMartAPI.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace AlutaMartAPI.Services;
    public class AuthService(IUnitOfWork unitOfWork, IResponseService responseService, 
	SignInManager<Profile> signInManager, UserManager<Profile> userManager, ILoggerFactory logger, INotificationService notificationService) 
	: BaseDBService(unitOfWork, responseService), IAuthService
{
	private readonly SignInManager<Profile> _signInManager = signInManager;
	public readonly UserManager<Profile> _userManager = userManager;
	protected readonly ILogger _logger = logger.CreateLogger("AuthService");
	private readonly INotificationService _notificationService = notificationService;

    public async Task<ServiceResponse<TokenResponseDTO>> EmailLoginAsync(EmailLoginDTO model)
	{
		model.Email = model.Email.TrimAllSpace();
		model.Password = model.Password.TrimAllSpace();
		if(!model.Email.IsValidEmail()) return _responseService.ErrorResponse<TokenResponseDTO>("Invalid email address");

		model.Email = model.Email.Trim().ToUpper();
		var profile = await _unitOfWork.Context.Profiles
			.AsNoTracking()
			.Where(x => x.NormalizedEmail == model.Email)
			.Select(x => new Profile
			{
				Email = x.Email,
				PasswordHash = x.PasswordHash,
				SecurityStamp = x.SecurityStamp,
				ConcurrencyStamp = x.ConcurrencyStamp,
				FirstName = x.FirstName,
				LastName = x.LastName,
				PhoneNumber = x.PhoneNumber,
				Id = x.Id,
				IsActive = x.IsActive,
				Role = x.Role,
			}).FirstOrDefaultAsync();
		if(profile == null) return _responseService.ErrorResponse<TokenResponseDTO>("Invalid email or password");

		var loginCheckResult = await _signInManager.UserManager.CheckPasswordAsync(profile, model.Password);
		if (!loginCheckResult) return _responseService.ErrorResponse<TokenResponseDTO>("Invalid email or password");

		return _responseService.SuccessResponse(await GetBearerTokenAsync(profile, 720));
	}

	public async Task<ServiceResponse<string>> SeedAdminAsync()
	{
		var isAdminExist = await _unitOfWork.Context.Profiles.AnyAsync(x => x.Role == Roles.SuperAdmin);
		if(isAdminExist)
		{
			_logger.LogInformation("system admin user already exist");
			return _responseService.SuccessResponse("admin user already exist");
		}

		var normalizedEmail = Constants.AdminEmail.ToUpper();
	   	var date = DateTime.UtcNow;
		var user = new Profile
		{
			FirstName = Constants.AdminFirstName,
			LastName = Constants.AdminLastName,
			ProfilePictureUrl = "https://pather-bucket.s3.eu-central-1.amazonaws.com/57aca2a0-32d8-4b05-8541-c776cdf81a1f.png",
			Email = normalizedEmail,
			EmailConfirmed = true,
			PhoneNumber = Constants.AdminPhone,
			UserName = normalizedEmail,
			Created = date, Modified = date,
			PhoneNumberConfirmed = true,
			Role = Roles.SuperAdmin,
			Token = 0,
			IsActive = true
		};

		var identityResult = await _userManager.CreateAsync(user);
		if(!identityResult.Succeeded)
		{
			return _responseService.ErrorResponse<string>(string.Join(",", identityResult
				.Errors
				.Select(x => x.Description)));
		}

		var addPasswordResp = await _userManager.AddPasswordAsync(user, Constants.AdminPassword);
		if(!addPasswordResp.Succeeded)
		{
			return _responseService.ErrorResponse<string>(string.Join(",",addPasswordResp
				.Errors
				.Select(x => x.Description)));
		}

		return _responseService.SuccessResponse("admin account seeded successfully..");
	}

	public async Task<ServiceResponse<string>> CreateNewPasswordAsync(CreatePasswordDTO model)
	{
		if(model.Password != model.ConfirmPassword) return _responseService.ErrorResponse<string>("Passwords do not match");

		var profile = await _userManager.Users.Where(x => x.Token == model.Token).FirstOrDefaultAsync();
		if(profile is null) return _responseService.ErrorResponse<string>("The code you've entered is invalid");

		if(profile.IsActive && (profile.TokenResetTime.Value - DateTime.UtcNow).TotalMinutes > 20) return _responseService.ErrorResponse<string>("The code you've entered has expired");

		var resetToken = await _userManager.GeneratePasswordResetTokenAsync(profile);
		var passwordChangeResult = await _userManager.ResetPasswordAsync(profile, resetToken, model.ConfirmPassword);
        
		if(!passwordChangeResult.Succeeded) return _responseService.ErrorResponse<string>(string.Join(",", passwordChangeResult.Errors.Select(x => x.Description)));

		await _unitOfWork.Context.Database.ExecuteSqlRawAsync(ProfileSQL.DeleteToken, new NpgsqlParameter("@id", profile.Id));
		
		_notificationService.UpdatePasswordEmailAsync(profile.Email, profile.FirstName).Forget();
		return _responseService.SuccessResponse("password created successfully..");
	}

	public async Task<ServiceResponse<string>> ResetPasswordAsync(ResetPasswordDTO model)
	{
		if(!model.Email.IsValidEmail()) return _responseService.ErrorResponse<string>("Invalid email address");

		var normalizedEmail = model.Email.Trim().ToUpper();
		var profile = await _unitOfWork.Context.Users
			.AsNoTracking()
			.Where(x => x.NormalizedEmail == normalizedEmail)
			.Select(x => new Profile
				{
					FirstName = x.FirstName,
					Id = x.Id,
					Token = x.Token,
					TokenResetTime = x.TokenResetTime
				})
			.FirstOrDefaultAsync();

		if(profile is null) return _responseService.ErrorResponse<string>($"user account with email {model.Email} does not exist");

		if(profile.Token <= 0 || !profile.TokenResetTime.HasValue || (DateTime.UtcNow - profile.TokenResetTime.Value).TotalMinutes > 10)
		{
			profile.Token = AppUtilities.RandomInt(6);
			var parameters = new List<object>
			{
				new NpgsqlParameter("@id", profile.Id),
				new NpgsqlParameter("@token", profile.Token)
			};
			await _unitOfWork.Context.Database.ExecuteSqlRawAsync(ProfileSQL.UpdateToken, parameters);
			_notificationService.ResetPasswordEmailAsync(model.Email, profile.FirstName, profile.Token.ToString()).Forget();
		}
		return _responseService.SuccessResponse("Password reset request was successful");
	}

	public async Task<ServiceResponse<string>> CreateAccountAsync(CreateUserDTO model, bool isLearner = false)
	{
		if(!model.Email.IsValidEmail()) return _responseService.ErrorResponse<string>("Invalid email");
		model.Password = model.Password.TrimAllSpace();

		var normalizedEmail = model.Email.TrimAllSpace().ToUpper();

		var isEmailExist = await _unitOfWork.Context.Profiles.AnyAsync(x => x.NormalizedEmail == normalizedEmail);
		if(isEmailExist) return _responseService.ErrorResponse<string>($"user with email '{model.Email}' already exist");

        var date = DateTime.UtcNow;
		var profile = new Profile
		{
			FirstName = model.FirstName.ToLower(),
			LastName = model.LastName.ToLower(),
			Email = model.Email.ToLower(),
			UserName = normalizedEmail,
			Created = date,
			Modified = date,
			Role = Roles.Buyer,
			IsActive = false,
			EmailConfirmed = false,
			TokenResetTime = date,
			Token = AppUtilities.RandomInt(9)
		};

		List<string> passwordError = [];

		foreach(IPasswordValidator<Profile> passwordValidator in _userManager.PasswordValidators)
		{
			var checkResult = await passwordValidator.ValidateAsync(_userManager, profile, model.Password);
			if(!checkResult.Succeeded) passwordError.AddRange(checkResult.Errors.Select(x => x.Description));
		}

		if(passwordError.Count != 0) return _responseService.ErrorResponse<string>(string.Join(",", passwordError));

		var createResp = await _userManager.CreateAsync(profile);
		if(!createResp.Succeeded) return _responseService.ErrorResponse<string>(string.Join(",", createResp.Errors.Select(x => x.Description)));

		await _userManager.AddPasswordAsync(profile, model.Password);

		_notificationService.UserEmailVerificationAsync(model.Email, model.FirstName, profile.Token.ToString(), isLearner).Forget();
		return _responseService.SuccessResponse("user account created successfully...");
	}

	public async Task<ServiceResponse<string>> ResendEmailVerificationAsync(UserDTO user, bool isLearner = false)
	{
		var profile = await _unitOfWork.Context.Profiles
			.AsNoTracking()
			.Where(x => x.Id == user.Id)
			.Select(x => new
			{
				x.Token,
				x.TokenResetTime,
				x.EmailConfirmed
			})
			.FirstOrDefaultAsync();
		
		if(profile is null) return _responseService.ErrorResponse<string>("Invalid request");

		if(profile.EmailConfirmed) return _responseService.ErrorResponse<string>("User email is already verified");

        if(profile.Token <= 0 || !profile.TokenResetTime.HasValue || (DateTime.UtcNow - profile.TokenResetTime.Value).TotalMinutes > 20)
        {
			var token = AppUtilities.RandomLong(9);
			_notificationService.UserEmailVerificationAsync(user.Email, user.FirstName, token.ToString(), isLearner).Forget();
			var parameters = new List<object>
			{
				new NpgsqlParameter("@id", user.Id),
				new NpgsqlParameter("@token", token)
			};
			await _unitOfWork.Context.Database.ExecuteSqlRawAsync(ProfileSQL.UpdateToken, parameters);
        }
		return _responseService.SuccessResponse("email verification sent successfully..");
	}

	public async Task<ServiceResponse<string>> VerifiyAccountAsync(int token)
	{
		if(token.IntCount() != 9) return _responseService.ErrorResponse<string>("verification code entered is invalid");

		var isProfileExist = await _unitOfWork.Context.Profiles.AnyAsync(x => x.Token == token);
		if(!isProfileExist) return _responseService.ErrorResponse<string>("verification code entered is invalid");

		await _unitOfWork.Context.Database.ExecuteSqlRawAsync(ProfileSQL.VerifyUserAccount, new NpgsqlParameter("@token", token));
		return _responseService.SuccessResponse("User account verified successfully...");
	}

	public async Task<ServiceResponse<string>> SetPasswordAsync(int token, SetPasswordDTO model)
	{
		if(token.ToString().Length != 9) return _responseService.ErrorResponse<string>("Invalid token");

		var profile = await _userManager.Users.Where(x => x.Token == token).FirstOrDefaultAsync();
		if (profile is null) return _responseService.ErrorResponse<string>("Invalid token");

		if ((DateTime.UtcNow - profile.TokenResetTime.Value).TotalMinutes > 1440) return _responseService.ErrorResponse<string>("Link has expired");

		List<string> passwordErrors = new List<string>();

		foreach (IPasswordValidator<Profile> passwordValidator in _userManager.PasswordValidators)
		{
			var checkResult = await passwordValidator.ValidateAsync(_userManager, profile, model.Password);
			if (!checkResult.Succeeded) passwordErrors.AddRange(checkResult.Errors.Select(x => x.Description));
		}

		if (passwordErrors.Count > 0) return _responseService.ErrorResponse<string>(string.Join(",", passwordErrors));

		var resetToken = await _userManager.GeneratePasswordResetTokenAsync(profile);
		var passwordChangeResult = await _userManager.ResetPasswordAsync(profile, resetToken, model.ConfirmPassword);

		if (!passwordChangeResult.Succeeded) return _responseService.ErrorResponse<string>(string.Join(",", passwordChangeResult.Errors.Select(x => x.Description)));

		await _unitOfWork.Context.Database.ExecuteSqlRawAsync(ProfileSQL.DeleteToken, new NpgsqlParameter("@id", profile.Id));

		_notificationService.SetPasswordSuccessEmailAsync(profile.Email, profile.FirstName, profile.Role).Forget();

		return _responseService.SuccessResponse("Password created successfully.");
	}
}
using AlutaMartAPI.Database;
using AlutaMartAPI.DTOs;
using AlutaMartAPI.Models;
using AlutaMartAPI.SQLQueries;
using AlutaMartAPI.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace AlutaMartAPI.Services;
public class AdminService(IUnitOfWork _unitOfWork, IResponseService _responseService, INotificationService notificationService, UserManager<Profile> userManager) : IAdminService
{
    public readonly UserManager<Profile> _userManager = userManager;
	private readonly INotificationService _notificationService = notificationService;
    public async Task<ServiceResponse<string>> CreateAdminAsync(CreateAdminDTO model)
    {
        if(!model.Email.IsValidEmail()) return _responseService.ErrorResponse<string>("Invalid email");
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
			Role = model.AdminRole,
			IsActive = true,
			EmailConfirmed = true,
			TokenResetTime = date,
			Token = AppUtilities.RandomInt(9)
		};
        var createResp = await _userManager.CreateAsync(profile);
		if(!createResp.Succeeded) return _responseService.ErrorResponse<string>(string.Join(",", createResp.Errors.Select(x => x.Description)));

        var parameters = new List<object>
		{
			new NpgsqlParameter("@id", profile.Id),
			new NpgsqlParameter("@token", profile.Token)
		};
		await _unitOfWork.Context.Database.ExecuteSqlRawAsync(ProfileSQL.UpdateToken, parameters);
		_notificationService.SetPasswordEmailAsync(model.Email, profile.FirstName, model.AdminRole, profile.Token.ToString()).Forget();
		
		return _responseService.SuccessResponse($"{model.AdminRole.Name()} created and set password link sent successful");
    }}
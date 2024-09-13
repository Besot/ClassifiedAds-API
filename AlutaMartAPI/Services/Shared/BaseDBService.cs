using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AlutaMartAPI.Database;
using AlutaMartAPI.DTOs;
using AlutaMartAPI.Models;
using AlutaMartAPI.SQLQueries;
using AlutaMartAPI.Utilities;
using Bugsnag.Payload;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Npgsql;

namespace AlutaMartAPI.Services;

public abstract class BaseDBService(IUnitOfWork unitOfWork, IResponseService responseService)
{
	protected readonly IUnitOfWork _unitOfWork = unitOfWork;
	protected readonly IResponseService _responseService = responseService;

    public TokenResponseDTO GetBearerToken(Profile model, int tokenValidity)
	{
		var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Constants.JWTKey)) { KeyId = Constants.JWTKeyId };
		var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512);

		var profile = new UserDTO
		{
			Id = model.Id,
			FirstName = model.FirstName,
			LastName = model.LastName,
			Email = model.Email,
			Phone = model.PhoneNumber,
			Access = model.Role,
			IsActive = model.IsActive,
		};

		var claims = new List<Claim>
		{
			new(JwtRegisteredClaimNames.Sub, profile.ToJson()),
			new(JwtRegisteredClaimNames.Aud, model.Role.ToString())
		};

		var date = DateTime.UtcNow;
		var expiryDate = date.AddHours(tokenValidity) - date;
		var tokenDescriptor = new SecurityTokenDescriptor
		{
			Subject = new ClaimsIdentity(claims),
			Expires = date.AddHours(tokenValidity),
			SigningCredentials = credentials,
			Audience = Constants.JWTIssuerAndAudience,
			Issuer = Constants.JWTIssuerAndAudience,
			NotBefore = date,
		};

		var jwtTokenHandler = new JwtSecurityTokenHandler();
		var token = jwtTokenHandler.CreateToken(tokenDescriptor);

		return new TokenResponseDTO
		{
			AccessToken = jwtTokenHandler.WriteToken(token),
			TokenExp = Convert.ToInt32(expiryDate.TotalSeconds),
		};
	}

	public async Task<TokenResponseDTO> GetBearerTokenAsync(Profile model, int tokenValidity)
	{
		var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Constants.JWTKey)) { KeyId = Constants.JWTKeyId };
		var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512);

		var profile = new UserDTO
		{
			Id = model.Id,
			FirstName = model.FirstName,
			LastName = model.LastName,
			Email = model.Email,
			Phone = model.PhoneNumber,
			Access = model.Role,
			IsActive = model.IsActive,
		};

		if (model.Role == Roles.Vendor)
    	{
        var vendorData = await _unitOfWork.Context.Vendors
            .AsNoTracking()
            .Where(x => x.ProfileId == model.Id)
            .Select(x => new
            {
                VendorId = x.Id,
                x.VendorPictureUrl,
                x.VendorPlanTier.PlanTierId,
                PlanTierName = x.VendorPlanTier.PlanTier.Name
            })
            .FirstOrDefaultAsync();

        if (vendorData != null)
        {
            profile.ProfilePicUrl = vendorData.VendorPictureUrl;
            profile.VendorId = vendorData.VendorId;
            profile.VendorPlanTierId = vendorData.PlanTierId;
            profile.VendorPlanTier = vendorData.PlanTierName;
        }
		}
		else if (model.Role == Roles.Buyer)
		{
			var buyerData = await _unitOfWork.Context.Buyers
				.AsNoTracking()
				.Where(x => x.ProfileId == model.Id)
				.Select(x => new
				{
					BuyerId = x.Id,
					ProfilePicUrl = x.Profile.ProfilePictureUrl
				})
				.FirstOrDefaultAsync();

			if (buyerData != null)
			{
				profile.ProfilePicUrl = buyerData.ProfilePicUrl;
				profile.BuyerId = buyerData.BuyerId;
			}
		}


		var claims = new List<Claim>
		{
			new(JwtRegisteredClaimNames.Sub, profile.ToJson()),
			new(JwtRegisteredClaimNames.Aud, model.Role.ToString())
		};

		var date = DateTime.UtcNow;
		var expiryDate = date.AddHours(tokenValidity) - date;
		var tokenDescriptor = new SecurityTokenDescriptor
		{
			Subject = new ClaimsIdentity(claims),
			Expires = date.AddHours(tokenValidity),
			SigningCredentials = credentials,
			Audience = Constants.JWTIssuerAndAudience,
			Issuer = Constants.JWTIssuerAndAudience,
			NotBefore = date,
		};

		var jwtTokenHandler = new JwtSecurityTokenHandler();
		var token = jwtTokenHandler.CreateToken(tokenDescriptor);

		return new TokenResponseDTO
		{
			AccessToken = jwtTokenHandler.WriteToken(token),
			TokenExp = Convert.ToInt32(expiryDate.TotalSeconds),
		};
	}

	public async Task AdEngagementAsync(Guid adId, Guid profileId, bool isEnrolling = false)
    {
        var adEngagement = await _unitOfWork.Context.AdsEngagements
            .FirstOrDefaultAsync(x => x.AdId == adId && x.ProfileId == profileId);

        if (adEngagement == null)
        {
            adEngagement = new AdsEngagement
            {
                AdId = adId,
                ProfileId = profileId,
                VisitCount = 1,
                IsEnrolled = isEnrolling
            };
            await _unitOfWork.Context.AdsEngagements.AddAsync(adEngagement);
            await _unitOfWork.CommitAsync();
        }
        else
        {
            adEngagement.VisitCount += 1;

            var parameters = new[]
            {
                new NpgsqlParameter("@VisitCount", adEngagement.VisitCount),
                new NpgsqlParameter("@IsEnrolled", isEnrolling),
                new NpgsqlParameter("@CourseId", adId),
                new NpgsqlParameter("@ProfileId", profileId)
            };

            await _unitOfWork.Context.Database.ExecuteSqlRawAsync(AdSQL.UpdateAdEngagement, parameters);
        }
    }
}
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AlutaMartAPI.Database;
using AlutaMartAPI.DTOs;
using AlutaMartAPI.Models;
using AlutaMartAPI.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

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

		if(model.Role == Roles.Vendor)
		{
			var vendor = await _unitOfWork.Context.Vendors
				.AsNoTracking()
				.Where(x => x.ProfileId == model.Id)
				.Select(x => new { x.Id, x.ProfilePictureUrl })
				.FirstOrDefaultAsync();
			
			profile.ProfilePicUrl = vendor.ProfilePictureUrl;
			profile.ExpertId = vendor.Id;
			
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
}
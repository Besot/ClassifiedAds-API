using System.IdentityModel.Tokens.Jwt;
using AlutaMartAPI.DTOs;
using AlutaMartAPI.Services;
using AlutaMartAPI.Utilities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AlutaMartAPI.Controllers;[ApiController]
[Route("v1/[controller]")]
[EnableCors("CoresPolicy")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class BaseController : Controller
{
	#region Validation Message

	public override void OnActionExecuting(ActionExecutingContext context)
	{
		if (!ModelState.IsValid)
		{
			var message = string.Join(", ", ModelState.Values.SelectMany(x => x.Errors).Select(err => err.ErrorMessage));
			context.Result = Ok(new ResponseService().ErrorResponse<string>(message));
		}
		base.OnActionExecuting(context);
	}

	#endregion

	public UserDTO CurrentUser => UserFromToken();

	private UserDTO UserFromToken()
	{
		var handler = new JwtSecurityTokenHandler();
		string headerData = Request.Headers.Authorization.ToString().Replace("Bearer", string.Empty).Trim();

		if(!string.IsNullOrEmpty(headerData))
		{
			var token = (JwtSecurityToken)handler.ReadToken(headerData);
			return token.Claims.First(claim => claim.Type == "sub").Value.FromJson<UserDTO>();
		}
		return new UserDTO();
	}
}
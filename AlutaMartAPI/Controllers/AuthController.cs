using AlutaMartAPI.DTOs;
using AlutaMartAPI.Services;
using AlutaMartAPI.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AlutaMartAPI.Controllers;
[AllowAnonymous]
public class AuthController(IAuthService authService) : BaseController
{
	[HttpPost("Login")]
	[ProducesResponseType(type: typeof(ServiceResponse<TokenResponseDTO>), statusCode: 200)]
	public async Task<IActionResult> EmailLogin([FromBody] EmailLoginDTO model) => Ok(await  authService.EmailLoginAsync(model));

	[HttpGet("Seed-Admin")]
	[ApiExplorerSettings(IgnoreApi = true)]
	[ProducesResponseType(type: typeof(ServiceResponse<TokenResponseDTO>), statusCode: 200)]
	public async Task<IActionResult> SeedAdmin() => Ok(await  authService.SeedAdminAsync());

    [HttpPost("Create-Account")]
	[ProducesResponseType(type: typeof(ServiceResponse<string>), statusCode: 200)]
	public async Task<IActionResult> SignUpUser([FromBody] CreateUserDTO model, bool isLearner = false) 
		=> Ok(await  authService.CreateAccountAsync(model, isLearner));

	[HttpPost("Verify-Account/{code}")]
	[ProducesResponseType(type: typeof(ServiceResponse<string>), statusCode: 200)]
	public async Task<IActionResult> VerifyAccount(int code) => Ok(await  authService.VerifiyAccountAsync(code));

	[HttpPost("Resend-Email-Verification")]
	[ProducesResponseType(type: typeof(ServiceResponse<string>), statusCode: 200)]
	public async Task<IActionResult> ResendVerification(bool isLearner = false) 
		=> Ok(await  authService.ResendEmailVerificationAsync(CurrentUser, isLearner));

	[HttpPost("Forgot-Password")]
	[ProducesResponseType(type: typeof(ServiceResponse<string>), statusCode: 200)]
	public async Task<IActionResult> ForgotPassword([FromBody] ResetPasswordDTO model) => Ok(await  authService.ResetPasswordAsync(model));

	[HttpPost("Create-Password")]
	[ProducesResponseType(type: typeof(ServiceResponse<string>), statusCode: 200)]
	public async Task<IActionResult> CreatePassword([FromBody] CreatePasswordDTO model) => Ok(await  authService.CreateNewPasswordAsync(model));

	[HttpPost("Set-Password/{code}")]
	[ProducesResponseType(type: typeof(ServiceResponse<string>), statusCode: 200)]
	public async Task<IActionResult> SetPassword(int code, [FromBody] SetPasswordDTO model) => Ok(await  authService.SetPasswordAsync(code, model));

}

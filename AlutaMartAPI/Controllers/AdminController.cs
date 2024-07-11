using AlutaMartAPI.DTOs;
using AlutaMartAPI.Services;
using AlutaMartAPI.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace AlutaMartAPI.Controllers;
    public class AdminController(IAdminService adminService) :BaseController
    {
	[HttpPost("Create"), AllowAccess(Roles.SuperAdmin)]
	[ProducesResponseType(type: typeof(ServiceResponse<string>), statusCode: 200)]
	public async Task<IActionResult> CreateAdmin([FromBody] CreateAdminDTO model) => Ok(await adminService.CreateAdminAsync(model));
}
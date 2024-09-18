using AlutaMartAPI.DTOs;
using AlutaMartAPI.ModelObjects;
using AlutaMartAPI.Services;
using AlutaMartAPI.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace AlutaMartAPI.Controllers;
    public class AdminController(IAdminService adminService) :BaseController
    {
	[HttpPost("Create"), AllowAccess(Roles.SuperAdmin)]
	[ProducesResponseType(type: typeof(ServiceResponse<string>), statusCode: 200)]
	public async Task<IActionResult> CreateAdmin([FromBody] CreateAdminDTO model) 
		=> Ok(await adminService.CreateAdminAsync(model));

	[HttpPatch("SetProfileState/{profileId}"), AllowAccess(Roles.SuperAdmin)]
    [ProducesResponseType(type: typeof(ServiceResponse<string>), statusCode: 200)]
    public async Task<IActionResult> SetAdminProfileState(Guid profileId, [FromQuery] bool isActive)  
		=> Ok(await adminService.SetAdminProfileStateAsync(profileId, isActive));

	[HttpGet, AllowAccess(Roles.SuperAdmin)]
	[ProducesResponseType(type: typeof(ServiceResponse<PagedList<GetAdminDTO>>), statusCode: 200)]
	public async Task<IActionResult> GetAdmins(int page = 1, int pageSize = 15) 
		=> Ok(await adminService.GetAsync( page, pageSize));

	[HttpGet("{profileId}"), AllowAccess(Roles.SuperAdmin)]
	[ProducesResponseType(type: typeof(ServiceResponse<GetAdminDTO>), statusCode: 200)]
	public async Task<IActionResult> GetById(Guid profileId) => Ok(await adminService.GetByAdminIdAsync(profileId));
}
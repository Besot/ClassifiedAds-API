using AlutaMartAPI.DTOs;
using AlutaMartAPI.Services;
using AlutaMartAPI.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace AlutaMartAPI.Controllers;
    public class VendorController(IVendorService _vendorService) :BaseController
    {
        [HttpPost("Create")]
	    [ProducesResponseType(type: typeof(ServiceResponse<string>), statusCode: 200)]
	    public async Task<IActionResult> Create([FromBody] CreateVendorDTO model) => Ok(await _vendorService.CreateAsync(model, CurrentUser));
   
        [HttpGet, AllowAccess(Roles.SuperAdmin)]
        [ProducesResponseType(type: typeof(ServiceResponse<PagedList<GetVendorDTO>>), statusCode: 200)]
        public async Task<IActionResult> GetVendor(int page = 1, int pageSize = 15) => Ok(await _vendorService.GetAsync( page, pageSize));

        [HttpGet("Details/{vendorId}")]
        [ProducesResponseType(type: typeof(ServiceResponse<GetVendorDTO>), statusCode: 200)]
        public async Task<IActionResult> GetDetails(Guid vendorId) => Ok(await _vendorService.GetDetailsAsync(vendorId));

        [HttpDelete("Delete-Expert/{profileId}"), AccessControl([Roles.AdminUser, Roles.SuperAdmin], AccessType.Allow)]
        [ProducesResponseType(type: typeof(ServiceResponse<string>), statusCode: 200)]
        public async Task<IActionResult> DeleteVendor(Guid profileId)  => Ok(await _vendorService.DeleteVendorAsync(profileId));
    }
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
   
    }
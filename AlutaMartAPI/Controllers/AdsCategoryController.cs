using AlutaMartAPI.DTOs;
using AlutaMartAPI.Services;
using AlutaMartAPI.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace AlutaMartAPI.Controllers;
    public class AdsCategoryController(IAdsCategoryService _adsCategoryService) :BaseController
    {
	[HttpGet]
	[ProducesResponseType(type: typeof(ServiceResponse<List<GetAdsCategoryDTO>>), statusCode: 200)]
	public async Task<IActionResult> Get() => Ok(await _adsCategoryService.GetAsync());
}
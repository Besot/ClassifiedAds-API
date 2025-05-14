using AlutaMartAPI.DTOs;
using AlutaMartAPI.Services;
using AlutaMartAPI.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AlutaMartAPI.Controllers;
    public class AdsCategoryController(ICategoryService _adsCategoryService) :BaseController
    {
		[HttpGet, AllowAnonymous]
		[ProducesResponseType(type: typeof(ServiceResponse<List<GetAdsCategoryDTO>>), statusCode: 200)]
		public async Task<IActionResult> Get() => Ok(await _adsCategoryService.GetAsync());
	}
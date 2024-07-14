using AlutaMartAPI.DTOs;
using AlutaMartAPI.Services;
using AlutaMartAPI.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace AlutaMartAPI.Controllers;
    public class InstitutionController(IInstitutionService institutionService) : BaseController
    {
	[HttpGet]
	[ProducesResponseType(type: typeof(ServiceResponse<List<GetInstitutionDTO>>), statusCode: 200)]
	public async Task<IActionResult> Get() => Ok(await institutionService.GetAsync());
}
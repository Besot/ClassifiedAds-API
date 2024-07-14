using AlutaMartAPI.ModelObjects;
using AlutaMartAPI.Services;
using AlutaMartAPI.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace AlutaMartAPI.Controllers;
    public class CurrencyController(ICurrencyService currencyService) :BaseController
    {
	[HttpGet]
	[ProducesResponseType(type: typeof(ServiceResponse<List<GetCurrencyDTO>>), statusCode: 200)]
	public async Task<IActionResult> Get() => Ok(await currencyService.GetAsync());
    }
using AlutaMartAPI.DTOs;
using AlutaMartAPI.Services;
using AlutaMartAPI.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace AlutaMartAPI.Controllers;
    public class AdsController(IAdsService adsService) :BaseController
    {
        [HttpPost("Create"), AllowAccess(Roles.Vendor)]
        [ProducesResponseType(type: typeof(ServiceResponse<string>), statusCode: 200)]
        public async Task<IActionResult> CreateAds([FromBody] CreateAdsDTO model) 
            => Ok(await adsService.CreateAdsAsync(model, CurrentUser));

    }
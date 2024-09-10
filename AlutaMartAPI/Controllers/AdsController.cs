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

        [HttpGet]
        [ProducesResponseType(type: typeof(ServiceResponse<PagedList<GetAdsDTO>>), statusCode: 200)]
        public async Task<IActionResult> GetAds(int page = 1, int pageSize = 15) => Ok(await adsService.GetAsync( page, pageSize));

        [HttpGet("Get/Vendor"), AllowAccess(Roles.Vendor)]
        [ProducesResponseType(type: typeof(ServiceResponse<PagedList<GetAdsDTO>>), statusCode: 200)]
        public async Task<IActionResult> GetAdByVendor(int page = 1, int pageSize = 20) 
            => Ok(await adsService.GetByVendorIdAsync(CurrentUser.VendorId.Value, page, pageSize));

    }
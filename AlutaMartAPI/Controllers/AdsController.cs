using AlutaMartAPI.DTOs;
using AlutaMartAPI.Services;
using AlutaMartAPI.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AlutaMartAPI.Controllers;
    public class AdsController(IAdsService adsService) :BaseController
    {
        [HttpPost("Create"), AllowAccess(Roles.Vendor)]
        [ProducesResponseType(type: typeof(ServiceResponse<string>), statusCode: 200)]
        public async Task<IActionResult> CreateAds([FromBody] CreateAdsDTO model) 
            => Ok(await adsService.CreateAdsAsync(model, CurrentUser));

        [HttpGet, AllowAnonymous]
        [ProducesResponseType(type: typeof(ServiceResponse<PagedList<GetAdsDTO>>), statusCode: 200)]
        public async Task<IActionResult> GetAds(int page = 1, int pageSize = 15) => Ok(await adsService.GetAsync( page, pageSize));

        [HttpGet("Get/Vendor"), AllowAccess(Roles.Vendor)]
        [ProducesResponseType(type: typeof(ServiceResponse<PagedList<GetAdsDTO>>), statusCode: 200)]
        public async Task<IActionResult> GetAdByVendor(int page = 1, int pageSize = 20) 
            => Ok(await adsService.GetByVendorIdAsync(CurrentUser.VendorId.Value, page, pageSize));

        [HttpGet("Details/{adId}"), AllowAnonymous]
        [ProducesResponseType(type: typeof(ServiceResponse<GetAdDetailsDTO>), statusCode: 200)]
        public async Task<IActionResult> GetDetails(Guid adId) => Ok(await adsService.GetDetailsAsync(adId, CurrentUser));

        [HttpGet("Search"), AllowAnonymous]
        [ProducesResponseType(type: typeof(ServiceResponse<PagedList<GetAdsDTO>>), statusCode: 200)]
        public async Task<IActionResult> SearchAds(string searchQuery = "", Guid? adsCategoryId = null, int page = 1, int pageSize = 20, bool? isFree = null)
            => Ok(await adsService.SearchAsync(searchQuery, adsCategoryId, page, pageSize, isFree));

        [HttpPut("Update-Ads/{adId}"), AllowAccess(Roles.Vendor)]
        [ProducesResponseType(type: typeof(ServiceResponse<string>), statusCode: 200)]
        public async Task<IActionResult> UpdateAd([FromRoute] Guid adId, [FromBody] CreateAdsDTO model)
            => Ok(await adsService.UpdateAdAsync(adId, model, CurrentUser));

    }
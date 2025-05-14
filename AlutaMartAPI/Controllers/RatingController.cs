using AlutaMartAPI.DTOs;
using AlutaMartAPI.Services;
using AlutaMartAPI.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace AlutaMartAPI.Controllers
{
    public class RatingController(IRatingService ratingService) : BaseController
    {
        [HttpPost, AllowAccess(Roles.Buyer)]
        [ProducesResponseType(typeof(ServiceResponse<string>), 200)]
        public async Task<IActionResult> CreateRating([FromBody] CreateRatingDTO model)
            => Ok(await ratingService.CreateRatingAsync(model, CurrentUser));

        [HttpGet("Vendor/{vendorId}"), AllowAnonymous]
        [ProducesResponseType(typeof(ServiceResponse<PagedList<GetRatingDTO>>), 200)]
        public async Task<IActionResult> GetVendorRatings(Guid vendorId, int page = 1, int pageSize = 10)
            => Ok(await ratingService.GetRatingsForVendorAsync(vendorId, page, pageSize));

        [HttpGet("Ad/{adId}"), AllowAnonymous]
        [ProducesResponseType(typeof(ServiceResponse<PagedList<GetRatingDTO>>), 200)]
        public async Task<IActionResult> GetAdRatings(Guid adId, int page = 1, int pageSize = 10)
            => Ok(await ratingService.GetRatingsForAdAsync(adId, page, pageSize));

        [HttpGet("Vendor/Average/{vendorId}"), AllowAnonymous]
        [ProducesResponseType(typeof(ServiceResponse<double>), 200)]
        public async Task<IActionResult> GetVendorAverageRating(Guid vendorId)
            => Ok(await ratingService.GetAverageRatingForVendorAsync(vendorId));

        [HttpGet("Ad/Average/{adId}"), AllowAnonymous]
        [ProducesResponseType(typeof(ServiceResponse<double>), 200)]
        public async Task<IActionResult> GetAdAverageRating(Guid adId)
            => Ok(await ratingService.GetAverageRatingForAdAsync(adId));
    }
}

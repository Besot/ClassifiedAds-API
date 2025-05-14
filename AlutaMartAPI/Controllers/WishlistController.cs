using AlutaMartAPI.DTOs;
using AlutaMartAPI.Services;
using AlutaMartAPI.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace AlutaMartAPI.Controllers
{
    public class WishlistController(IWishlistService wishlistService) : BaseController
    {
        [HttpGet, AllowAccess(Roles.Buyer)]
        [ProducesResponseType(typeof(ServiceResponse<GetWishlistDTO>), 200)]
        public async Task<IActionResult> GetWishlist()
            => Ok(await wishlistService.GetWishlistAsync(CurrentUser));

        [HttpPost("Add"), AllowAccess(Roles.Buyer)]
        [ProducesResponseType(typeof(ServiceResponse<string>), 200)]
        public async Task<IActionResult> AddToWishlist([FromBody] AddToWishlistDTO model)
            => Ok(await wishlistService.AddToWishlistAsync(model, CurrentUser));

        [HttpDelete("Remove/{itemId}"), AllowAccess(Roles.Buyer)]
        [ProducesResponseType(typeof(ServiceResponse<string>), 200)]
        public async Task<IActionResult> RemoveFromWishlist(Guid itemId)
            => Ok(await wishlistService.RemoveFromWishlistAsync(itemId, CurrentUser));

        [HttpDelete("Clear"), AllowAccess(Roles.Buyer)]
        [ProducesResponseType(typeof(ServiceResponse<string>), 200)]
        public async Task<IActionResult> ClearWishlist()
            => Ok(await wishlistService.ClearWishlistAsync(CurrentUser));
    }
}

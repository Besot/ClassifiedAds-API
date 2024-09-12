using AlutaMartAPI.DTOs;
using AlutaMartAPI.Services;
using AlutaMartAPI.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AlutaMartAPI.Controllers;
    public class CartController(ICartService cartService) : BaseController
    {
        [HttpPost("AddToCart"), AllowAnonymous]
        [ProducesResponseType(type: typeof(ServiceResponse<string>), statusCode: 200)]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartDTO model)
            => Ok(await cartService.AddToCartAsync(model, CurrentUser));

        [HttpGet("GetByProfileId")]
        [ProducesResponseType(type: typeof(ServiceResponse<PagedList<GetCartDTO>>), statusCode: 200)]
        public async Task<IActionResult> GetCartById(int page = 1, int pageSize = 15)
            => Ok(await cartService.GetCartByIdAsync(CurrentUser, page, pageSize));

        [HttpDelete("Remove-Qty-FromCart")]
        [ProducesResponseType(type: typeof(ServiceResponse<string>), statusCode: 200)]
        public async Task<IActionResult> RemoveFromCart([FromBody] RemoveFromCartDTO model)
            => Ok(await cartService.RemoveFromCartAsync(model, CurrentUser));

        [HttpDelete("DeleteCart/{cartId}"), AllowAccess(Roles.Buyer)]
        [ProducesResponseType(type: typeof(ServiceResponse<string>), statusCode: 200)]
        public async Task<IActionResult> DeleteCart(Guid cartId)
            => Ok(await cartService.DeleteCartAsync(cartId, CurrentUser));
    }
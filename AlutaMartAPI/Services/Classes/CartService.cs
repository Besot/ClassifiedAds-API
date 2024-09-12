using AlutaMartAPI.Database;
using AlutaMartAPI.DTOs;
using AlutaMartAPI.Models;
using AlutaMartAPI.Utilities;
using Microsoft.EntityFrameworkCore;

namespace AlutaMartAPI.Services;
public class CartService(IUnitOfWork _unitOfWork, IResponseService _responseService) : ICartService
{
    public async Task<ServiceResponse<string>> AddToCartAsync(AddToCartDTO model, UserDTO user)
    {
        // Validate buyer ID
        if (user.Id == Guid.Empty)
            return _responseService.ErrorResponse<string>("Invalid buyer request");

        // Validate the AdsId
        if (model.AdsId == Guid.Empty || !model.AdsId.HasValue)
            return _responseService.ErrorResponse<string>("Ads ID is required");

        // Fetch ad, vendor, and plan in one go
        var ads = await _unitOfWork.Context.Ads
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == model.AdsId.Value && a.QuantityInStock >= model.Quantity && !a.IsDeleted);
        
        

        if (ads == null)
            return _responseService.ErrorResponse<string>("Ad not found or out of stock");

        // Add the ad to the buyer's cart
        var cartItem = new Cart
        {
            AdsId = model.AdsId.Value,
            VendorId = ads.VendorId,
            BuyerId = user.BuyerId.Value,
            ProfileId = user.Id,
            QuantityAdded = model.Quantity,
            PriceAtTimeOfAdd = ads.DiscountPrice ?? ads.Price,
            IsActive = true
        };

        await _unitOfWork.Context.Carts.AddAsync(cartItem);
        await _unitOfWork.CommitAsync();

        return _responseService.SuccessResponse("Item added to cart successfully.");
    }

    public async Task<ServiceResponse<PagedList<GetCartDTO>>> GetCartByIdAsync(UserDTO user, int page = 1, int pageSize = 15)
    {
        if (user.Id == Guid.Empty)
            return _responseService.ErrorResponse<PagedList<GetCartDTO>>("Invalid request");

        // Fetch all cart items for the buyer
        var cartItems = _unitOfWork.Context.Carts
            .AsNoTracking()
            .OrderByDescending(x => x.Modified)
            .Where(c => c.ProfileId == user.Id && c.IsActive)
            .Select(c => new GetCartDTO
            {
                CartId = c.Id,
                AdsId = c.AdsId,
                AdsTitle = c.Ads.Title,
                Quantity = c.QuantityAdded,
                PriceAtTimeOfAdd = c.PriceAtTimeOfAdd,
                VendorId = c.VendorId,
                VendorName = c.Vendor.BrandName
            });

        return await _responseService.PagedResponseAsync(cartItems, page, pageSize, "CartItems");
    }

    public async Task<ServiceResponse<string>> RemoveFromCartAsync(RemoveFromCartDTO model, UserDTO user)
    {
        // Validate AdsId
        if (!model.AdsId.HasValue || model.AdsId == Guid.Empty)
            return _responseService.ErrorResponse<string>("Ads ID is required");

        // Validate Quantity
        if (model.Quantity <= 0)
            return _responseService.ErrorResponse<string>("Invalid quantity value");

        // Fetch the cart item
        var cartItem = await _unitOfWork.Context.Carts
            .FirstOrDefaultAsync(c => c.ProfileId == user.Id 
                                    && c.AdsId == model.AdsId.Value 
                                    && c.IsActive);

        if (cartItem == null)
            return _responseService.ErrorResponse<string>("Cart item not found");

        // Remove or update cart item based on the quantity
        if (model.Quantity >= cartItem.QuantityAdded)
        {
            _unitOfWork.Context.Carts.Remove(cartItem); // Remove the item if the quantity is greater or equal
        }
        else
        {
            cartItem.QuantityAdded -= model.Quantity; // Reduce the quantity
        }

        await _unitOfWork.CommitAsync(); // Save changes
        return _responseService.SuccessResponse("Cart item updated/removed successfully.");
    }

    public async Task<ServiceResponse<string>> DeleteCartAsync(Guid cartId, UserDTO user)
    {
        var cartItem = await _unitOfWork.Context.Carts
            .FirstOrDefaultAsync(c => c.Id == cartId && c.BuyerId == user.BuyerId.Value && c.IsActive);

        if (cartItem == null)return _responseService.ErrorResponse<string>("Cart item not found");

        // Remove the cart item
        _unitOfWork.Context.Carts.Remove(cartItem);
        await _unitOfWork.CommitAsync();

        return _responseService.SuccessResponse("Cart item deleted successfully.");
    }
}
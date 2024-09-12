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
}
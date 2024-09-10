using AlutaMartAPI.Database;
using AlutaMartAPI.DTOs;
using AlutaMartAPI.Models;
using AlutaMartAPI.Utilities;
using Microsoft.EntityFrameworkCore;

namespace AlutaMartAPI.Services;
public class AdsService(IUnitOfWork _unitOfWork, IResponseService _responseService) : BaseDBService(_unitOfWork, _responseService), IAdsService
{
    public async Task<ServiceResponse<string>> CreateAdsAsync(CreateAdsDTO model, UserDTO user)
    {
        // Validate the vendor ID
        if (user.VendorId == Guid.Empty || user.VendorId == null)
            return _responseService.ErrorResponse<string>("Invalid vendor request");

        // Validate the model's required fields
        if (model.CurrencyId == Guid.Empty || !model.CurrencyId.HasValue)
            return _responseService.ErrorResponse<string>("Currency type is required");

        if (model.AdsCategoryId == Guid.Empty || !model.AdsCategoryId.HasValue)
            return _responseService.ErrorResponse<string>("Ads category is required");

        // Validate image URL
        if (!model.AdsImageUrl.Contains("s3.eu-central-1.amazonaws.com"))
            return _responseService.ErrorResponse<string>("Invalid image URL");

        // Fetch vendor, plan tier, currency, and ads category in one go
        var vendor = await _unitOfWork.Context.Vendors
            .Include(v => v.PlanTier)
            .FirstOrDefaultAsync(v => v.Id == user.VendorId.Value);

        var isCurrencyValid = await _unitOfWork.Context.Currencies.AnyAsync(c => c.Id == model.CurrencyId.Value);
        var isAdsCategoryValid = await _unitOfWork.Context.AdsCategories.AnyAsync(ac => ac.Id == model.AdsCategoryId.Value);

        if (vendor == null || vendor.PlanTier == null)
            return _responseService.ErrorResponse<string>("Vendor or plan tier not found");

        if (!isCurrencyValid)
            return _responseService.ErrorResponse<string>("Currency selected is invalid");

        if (!isAdsCategoryValid)
            return _responseService.ErrorResponse<string>("Ads category selected is invalid");

        // Retrieve ad counts and validate against plan tier limits
        var adsCount = await _unitOfWork.Context.Ads.CountAsync(a => a.VendorId == user.VendorId.Value);
        var featuredAdsCount = await _unitOfWork.Context.Ads.CountAsync(a => a.VendorId == user.VendorId.Value && a.IsFeatured);

        if (adsCount >= vendor.PlanTier.MaxAds)
            return _responseService.ErrorResponse<string>("Ad creation limit reached for the current plan tier");

        var imageCount = model.AdsImageUrl.ToList().Count;
        if (imageCount > vendor.PlanTier.MaxPicture)
            return _responseService.ErrorResponse<string>("Number of pictures exceeds the maximum allowed for your plan tier");

        if (model.IsFeatured && featuredAdsCount >= vendor.PlanTier.MaxFeatured)
            return _responseService.ErrorResponse<string>("Featured ad limit reached for the current plan tier");

        // Create and save the ad
        var ad = new Ads
            {
                Title = model.Title,
                Description = model.Description,
                Amount = model.Price,
                DiscountPrice = model.DiscountPrice,
                AdsType = model.AdsType,
                VendorId = user.VendorId.Value,
                AdsCategoryId = model.AdsCategoryId.Value,
                CurrencyId = model.CurrencyId.Value,
                IsFeatured = model.IsFeatured,
                AdsCondition = model.AdsCondition
            };
            await _unitOfWork.Context.AddAsync(ad);
            var images = new List<AdsImage>();

        images.AddRange(model.AdsImageUrl.Select(x => new AdsImage
        {
            ImageUrl = x,
            AdsId = ad.Id
        }));

        await _unitOfWork.Context.AdsImages.AddRangeAsync(images);
        await _unitOfWork.CommitAsync();

        return _responseService.SuccessResponse(ad.Id.ToString());
    }

    public async Task<ServiceResponse<PagedList<GetAdsDTO>>> GetAsync(int page = 1, int pageSize = 15)
    {  
        var ads = _unitOfWork.Context.Ads
            .AsNoTracking()
            .Select(x => new GetAdsDTO
            {
                Id = x.Id,
                Title = x.Title,
                Description = x.Description,
                BrandName =  x.Vendor.BrandName,
                VendorImage = x.Vendor.VendorPictureUrl,
                Amount = x.Amount,
                DiscountPrice = x.DiscountPrice,
                AdsImageUrl = x.AdsImageUrl,
                ExpiryDate = x.ExpiryDate,
                Status = x.Status,
                IsFeatured = x.IsFeatured,
                AdsType = x.AdsType,
                AdsCondition = x.AdsCondition,
                NumberOfReviews = x.NumberOfReviews,
                VendorId = x.VendorId,
                AdsCategoryId = x.AdsCategoryId,
                CurrencyId = x.CurrencyId,
                Discount = x.Discount
            });
        return await _responseService.PagedResponseAsync(ads, page, pageSize, "Ads");
    }

}
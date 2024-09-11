using AlutaMartAPI.Database;
using AlutaMartAPI.DTOs;
using AlutaMartAPI.Models;
using AlutaMartAPI.SQLQueries;
using AlutaMartAPI.Utilities;
using Microsoft.EntityFrameworkCore;
using Npgsql;

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
        if (!model.AdsImageUrls.Contains("s3.eu-central-1.amazonaws.com"))
            return _responseService.ErrorResponse<string>("Invalid image URL");

        // Fetch vendor, plan tier, currency, and ads category in one go
        var vendor = await _unitOfWork.Context.Vendors
            .AsNoTracking()
            .FirstOrDefaultAsync(v => v.Id == user.VendorId.Value);

        var vendorPlan = await _unitOfWork.Context.VendorPlan
            .AsNoTracking()
            .FirstOrDefaultAsync(v => v.VendorId == user.VendorId.Value);

        var isCurrencyValid = await _unitOfWork.Context.Currencies.AnyAsync(c => c.Id == model.CurrencyId.Value);
        var isAdsCategoryValid = await _unitOfWork.Context.AdsCategories.AnyAsync(ac => ac.Id == model.AdsCategoryId.Value);

        if (vendor == null || vendorPlan == null)
            return _responseService.ErrorResponse<string>("Vendor or plan tier not found");

        if (!isCurrencyValid)
            return _responseService.ErrorResponse<string>("Currency selected is invalid");

        if (!isAdsCategoryValid)
            return _responseService.ErrorResponse<string>("Ads category selected is invalid");

        // Retrieve ad counts and validate against plan tier limits
        var adsCount = await _unitOfWork.Context.Ads.CountAsync(a => a.VendorId == user.VendorId.Value && !a.IsDeleted);
        var featuredAdsCount = await _unitOfWork.Context.Ads.CountAsync(a => a.VendorId == user.VendorId.Value && a.IsFeatured && !a.IsDeleted);

        if (adsCount >= vendorPlan.PlanTier.MaxAds)
            return _responseService.ErrorResponse<string>("Ad creation limit reached for the current plan tier");

        var imageCount = model.AdsImageUrls.ToList().Count;
        if (imageCount > vendorPlan.PlanTier.MaxPicture)
            return _responseService.ErrorResponse<string>("Number of pictures exceeds the maximum allowed for your plan tier");

        if (model.IsFeatured && featuredAdsCount >= vendorPlan.PlanTier.MaxFeatured)
            return _responseService.ErrorResponse<string>("Featured ad limit reached for the current plan tier");

        var ad = new Ads
        {
            Title = model.Title,
            Description = model.Description,
            Price = model.Price,
            QuantityInStock = model.QuantityInStock,
            DiscountPrice = model.DiscountPrice,
            AdsType = model.AdsType,
            VendorId = user.VendorId.Value,
            AdsCategoryId = model.AdsCategoryId.Value,
            CurrencyId = model.CurrencyId.Value,
            IsFeatured = model.IsFeatured,
            AdsCondition = model.AdsCondition,
            Status = AdsStatus.Active,
            Discount = model.DiscountPrice != null ? Discount.Discounted : Discount.FixedPrice,
            FeaturedExpiryDate =  vendorPlan.PlanTier.Name == "free tier" ? null :  DateTimeOffset.UtcNow.AddMonths(1)
        };
            await _unitOfWork.Context.AddAsync(ad);
            var images = new List<AdsImage>();

        images.AddRange(model.AdsImageUrls.Select(x => new AdsImage
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
            .OrderByDescending(x => x.Modified)
            .Where(x => x.Status == AdsStatus.Active)
            .Select(x => new GetAdsDTO
            {
                Id = x.Id,
                Title = x.Title,
                Description = x.Description,
                BrandName =  x.Vendor.BrandName,
                VendorImage = x.Vendor.VendorPictureUrl,
                Price = x.Price,
                DiscountPrice = x.DiscountPrice ?? 0,
                AdsImageUrl = _unitOfWork.Context.AdsImages
                .Where(x => x.AdsId == x.Id)
                .Select(x => x.ImageUrl)
                .FirstOrDefault(),
                Status = x.Status,
                IsFeatured = x.IsFeatured,
                AdsType = x.AdsType,
                AdsCondition = x.AdsCondition,
                NumberOfReviews = x.NumberOfReviews,
                VendorId = x.VendorId,
                AdCategoryId = x.AdsCategoryId,
                AdCategory = x.AdsCategory.Name,
                CurrencyId = x.CurrencyId,
                Discount = x.Discount
            });
        return await _responseService.PagedResponseAsync(ads, page, pageSize, "Ads");
    }

     public async Task<ServiceResponse<PagedList<GetAdsDTO>>> GetByVendorIdAsync(Guid vendorId, int page = 1, int pageSize = 10)
    {
        var ads = _unitOfWork.Context.Ads
            .AsNoTracking()
            .OrderByDescending(x => x.Modified)
            .Where(x => x.VendorId == vendorId)
            .Select(x => new GetAdsDTO
            {
                Id = x.Id,
                Title = x.Title,
                Description = x.Description,
                BrandName =  x.Vendor.BrandName,
                VendorImage = x.Vendor.VendorPictureUrl,
                Price = x.Price,
                DiscountPrice = x.DiscountPrice ?? 0,
                AdsImageUrl = _unitOfWork.Context.AdsImages
                .Where(x => x.AdsId == x.Id)
                .Select(x => x.ImageUrl)
                .FirstOrDefault(),
                IsFeatured = x.IsFeatured,
                AdCategoryId = x.AdsCategoryId,
                AdCategory = x.AdsCategory.Name,
                CurrencyId = x.CurrencyId,
                Discount = x.Discount
            });
        
        return await _responseService.PagedResponseAsync(ads, page, pageSize, "Ads");
    }

    public async Task<ServiceResponse<GetAdDetailsDTO>> GetDetailsAsync(Guid adId, UserDTO user)
    {
        var ad = await _unitOfWork.Context.Ads
            .AsNoTracking()
            .Where(x => x.Id == adId)
            .Select(x => new GetAdDetailsDTO
            {
                Id = x.Id,
                Title = x.Title,
                Description = x.Description,
                BrandName =  x.Vendor.BrandName,
                VendorImage = x.Vendor.VendorPictureUrl,
                Price = x.Price,
                QuantityInStock = x.QuantityInStock,
                DiscountPrice = x.DiscountPrice ?? 0,
                AdsImageUrl = _unitOfWork.Context.AdsImages
                    .Where(x => x.AdsId == x.Id)
                    .Select(x => x.ImageUrl)
                    .ToList(),                
                FeaturedExpiryDate = x.FeaturedExpiryDate,
                Status = x.Status,
                IsFeatured = x.IsFeatured,
                AdsType = x.AdsType,
                AdsCondition = x.AdsCondition,
                NumberOfReviews = x.NumberOfReviews,
                VendorId = x.VendorId,
                AdCategoryId = x.AdsCategoryId,
                AdCategory = x.AdsCategory.Name,
                CurrencyId = x.CurrencyId,
                Discount = x.Discount
            })
            .FirstOrDefaultAsync();
        
        if(ad is null) return _responseService.ErrorResponse<GetAdDetailsDTO>("Invalid request");

        ad.Reviews = await _unitOfWork.Context.Reviews
            .AsNoTracking()
            .Where(x => x.AdsId == adId)
            .Select(x => new GetVendorReviewDTO
            {
                ReviewId = x.Id,
                ReviewerName = $"{x.Buyer.Profile.FirstName} {x.Buyer.Profile.LastName}",
                Review = x.Content,
                ReviewerPicture = x.Buyer.Profile.ProfilePictureUrl,
                ReviewDate = x.Modified,
                Edited = x.Modified != x.Created
            })
            .ToListAsync();

        return _responseService.SuccessResponse(ad);
    }

    public async Task<ServiceResponse<PagedList<GetAdsDTO>>> SearchAsync(string searchQuery, Guid? adsCategoryId, int page, int pageSize, bool? isFree)
    {
        var adsQuery = _unitOfWork.Context.Ads
            .AsNoTracking()
            .OrderByDescending(x => x.Created)
            .Where(x => x.Status == AdsStatus.Active)
            .Select(x => new GetAdsDTO
            {
                Id = x.Id,
                Title = x.Title,
                Description = x.Description,
                BrandName =  x.Vendor.BrandName,
                VendorImage = x.Vendor.VendorPictureUrl,
                Price = x.Price,
                DiscountPrice = x.DiscountPrice ?? 0,
                AdsImageUrl = _unitOfWork.Context.AdsImages
                .Where(x => x.AdsId == x.Id)
                .Select(x => x.ImageUrl)
                .FirstOrDefault(),                
                AdCategoryId = x.AdsCategoryId,
                AdCategory = x.AdsCategory.Name,
                CurrencyId = x.CurrencyId,
                Discount = x.Discount
            });

        if(isFree is not null && isFree.Value) adsQuery = adsQuery.Where(x => x.Price == 0);
        
        if(isFree is not null && !isFree.Value) adsQuery = adsQuery.Where(x => x.Price > 0);
        
        adsQuery = adsCategoryId == Guid.Empty || adsCategoryId == null ? adsQuery : 
            adsQuery.Where(x => x.AdCategoryId == adsCategoryId.Value);

        adsQuery = string.IsNullOrEmpty(searchQuery) ? adsQuery :
            adsQuery.Where(x => x.Title.Contains(searchQuery) || x.Description.Contains(searchQuery));

        return await _responseService.PagedResponseAsync(adsQuery, page, pageSize, "Ads");
    }

    public async Task<ServiceResponse<string>> UpdateAdAsync(Guid adId, CreateAdsDTO model, UserDTO user)
    {
        if(user.VendorId == Guid.Empty || user.VendorId == null) return _responseService.ErrorResponse<string>("Invalid vendor request");

        var adVendorId = await _unitOfWork.Context.Ads.AsNoTracking().Where(x => x.Id == adId).Select(x => x.VendorId).FirstOrDefaultAsync();
        if(user.VendorId != adVendorId) return _responseService.ErrorResponse<string>("Unauthorized request");

        if(model.AdsImageUrls == null || model.AdsImageUrls.Count <= 0) return _responseService.ErrorResponse<string>("Image is required");
       
        if(model.CurrencyId == Guid.Empty || !model.CurrencyId.HasValue) return _responseService.ErrorResponse<string>("currency type is required");

        if(model.AdsCategoryId == Guid.Empty || !model.AdsCategoryId.HasValue) 
            return _responseService.ErrorResponse<string>("Ad category is required");

        var adExists = await _unitOfWork.Context.Ads.AnyAsync(x => x.Id == adId);
        if(!adExists) return _responseService.ErrorResponse<string>("Invalid Ad");

        var isValidCategory = await _unitOfWork.Context.AdsCategories.AnyAsync(x => x.Id == model.AdsCategoryId.Value);
        if(!isValidCategory) return _responseService.ErrorResponse<string>("Select a valid category");

        var isValidCurrency = await _unitOfWork.Context.Currencies.AnyAsync(x => x.Id == model.CurrencyId.Value);
        if(!isValidCurrency) return _responseService.ErrorResponse<string>("Select a valid currency");
        
        var discount = model.DiscountPrice == null ? Discount.FixedPrice : Discount.Discounted;

        var parameters = new List<object>
        {
            new NpgsqlParameter("@title", model.Title),
            new NpgsqlParameter("@description", model.Description),
            new NpgsqlParameter("@price", model.Price),
            new NpgsqlParameter("@quantityInStock", model.QuantityInStock),
            new NpgsqlParameter("@discountPrice", model.DiscountPrice),
            new NpgsqlParameter("@isFeatured", model.IsFeatured),
            new NpgsqlParameter("@adsType", model.AdsType),
            new NpgsqlParameter("@adsCondition", model.AdsCondition),
            new NpgsqlParameter("@adsCategoryId", model.AdsCategoryId),
            new NpgsqlParameter("@currencyId", model.CurrencyId),
            new NpgsqlParameter("@discount", discount)

        };

        await _unitOfWork.Context.Database.ExecuteSqlRawAsync(AdSQL.UpdateAd, parameters);


        var existingAdImages = await _unitOfWork.Context.AdsImages
                .AsNoTracking()
                .Where(x => x.AdsId == adId)
                .Select(x => x.ImageUrl)
                .ToListAsync();

            var newAdImages = model.AdsImageUrls
            .Except(existingAdImages)
            .Select(url => new AdsImage
            {
                AdsId = adId,
                ImageUrl = url,
            }).ToList();

            var outdatedImages = existingAdImages.Except(model.AdsImageUrls).ToList();
           
            if (newAdImages.Count > 0)
            {
                await _unitOfWork.Context.AdsImages.AddRangeAsync(newAdImages);
            }

            if (outdatedImages.Count > 0)
            {
                var deleteParameters = new List<object> 
                { 
                    new NpgsqlParameter("@adsId", adId) 
                };
                deleteParameters.AddRange(outdatedImages.Select((id, index) => new NpgsqlParameter($"@id{index}", id)));

                var deleteQuery = AdImageSQL.DeleteAdImages
                .Replace("{adimageIds}", string.Join(", ", outdatedImages.Select((_, index) => $"@id{index}")));
                
                await _unitOfWork.Context.Database.ExecuteSqlRawAsync(deleteQuery, deleteParameters);
            }

            await _unitOfWork.CommitAsync();
        return _responseService.SuccessResponse("Ad Updated Successfully");
    }

    public async Task<ServiceResponse<string>> DeleteAdAsync(Guid adId, Guid vendorId, bool isAdmin)
    {
            var ad = await _unitOfWork.Context.Ads
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == adId);

            if (ad == null) return _responseService.ErrorResponse<string>("Ad not found");

            if (!isAdmin && ad.VendorId != vendorId) return _responseService.ErrorResponse<string>("You do not have permission to delete this ad");

            await _unitOfWork.Context.Database.ExecuteSqlRawAsync(AdSQL.DeleteAd, new NpgsqlParameter("@adId", adId));

            await _unitOfWork.CommitAsync();

        return _responseService.SuccessResponse("Ad deleted successfully");
    }
}
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AlutaMartAPI.Database;
using AlutaMartAPI.DTOs;
using AlutaMartAPI.Models;
using AlutaMartAPI.Utilities;

namespace AlutaMartAPI.Services
{
    public class RatingService : IRatingService
    {
        private readonly IUnitOfWork      _unitOfWork;
        private readonly IResponseService _responseService;

        public RatingService(
            IUnitOfWork unitOfWork,
            IResponseService responseService)
        {
            _unitOfWork      = unitOfWork;
            _responseService = responseService;
        }

        public async Task<ServiceResponse<string>> CreateRatingAsync(CreateRatingDTO model, UserDTO currentUser)
        {
            // 1) Only buyers can rate
            if (currentUser.BuyerId == null)
                return _responseService.ErrorResponse<string>("Only buyers can submit ratings");

            // 2) Rating value check
            if (model.Rating < 1 || model.Rating > 5)
                return _responseService.ErrorResponse<string>("Rating must be between 1 and 5");

            // 3) Vendor vs Ad rating
            if (model.IsVendorRating)
            {
                // Vendor must exist
                bool vendorExists = await _unitOfWork.Context.Vendors
                    .AsNoTracking()
                    .AnyAsync(v => v.Id == model.TargetId);
                if (!vendorExists)
                    return _responseService.ErrorResponse<string>("Vendor not found");

                // Must have purchased from vendor
                bool hasPurchasedVendor = await _unitOfWork.Context.PurchasedAds
                    .AsNoTracking()
                    .AnyAsync(p => p.BuyerId == currentUser.BuyerId.Value && p.Ads.VendorId == model.TargetId);
                if (!hasPurchasedVendor)
                    return _responseService.ErrorResponse<string>("You can only rate vendors you have purchased from");

                // Upsert review
                var existingVendorReview = await _unitOfWork.Context.Set<Review>()
                    .FirstOrDefaultAsync(r => r.BuyerId == currentUser.BuyerId.Value && r.VendorId == model.TargetId);

                if (existingVendorReview != null)
                {
                    existingVendorReview.Content   = model.Comment;
                    existingVendorReview.Rating    = model.Rating;
                    existingVendorReview.Modified = DateTimeOffset.UtcNow;
                }
                else
                {
                    await _unitOfWork.Context.Reviews.AddAsync(new Review
                    {
                        Content   = model.Comment,
                        Rating    = model.Rating,
                        VendorId  = model.TargetId,
                        BuyerId   = currentUser.BuyerId.Value,
                        Created = DateTimeOffset.UtcNow
                    });
                }

                await _unitOfWork.CommitAsync();
                await UpdateVendorRatingAsync(model.TargetId);

                return _responseService.SuccessResponse("Vendor rating submitted/updated successfully");
            }
            else
            {
                // Ad must exist
                bool adExists = await _unitOfWork.Context.Ads
                    .AsNoTracking()
                    .AnyAsync(a => a.Id == model.TargetId);
                if (!adExists)
                    return _responseService.ErrorResponse<string>("Ad not found");

                // Must have purchased this ad
                bool hasPurchasedAd = await _unitOfWork.Context.PurchasedAds
                    .AsNoTracking()
                    .AnyAsync(p => p.BuyerId == currentUser.BuyerId.Value && p.AdId == model.TargetId);
                if (!hasPurchasedAd)
                    return _responseService.ErrorResponse<string>("You can only rate ads you have purchased");

                // Upsert review
                var existingAdReview = await _unitOfWork.Context.Reviews
                    .AsNoTracking()
                    .FirstOrDefaultAsync(r => r.BuyerId == currentUser.BuyerId.Value && r.AdsId == model.TargetId);

                if (existingAdReview != null)
                {
                    existingAdReview.Content   = model.Comment;
                    existingAdReview.Rating    = model.Rating;
                    existingAdReview.Modified = DateTimeOffset.UtcNow;
                }
                else
                {
                    await _unitOfWork.Context.Reviews.AddAsync(new Review
                    {
                        Content   = model.Comment,
                        Rating    = model.Rating,
                        AdsId     = model.TargetId,
                        BuyerId   = currentUser.BuyerId.Value,
                        Created = DateTimeOffset.UtcNow
                    });
                }

                await _unitOfWork.CommitAsync();

                return _responseService.SuccessResponse("Ad rating submitted/updated successfully");
            }
        }

        public async Task<ServiceResponse<double>> GetAverageRatingForAdAsync(Guid adId)
        {
            // 1) Ad existence
            bool adExists = await _unitOfWork.Context.Ads
                .AsNoTracking()
                .AnyAsync(a => a.Id == adId);
            if (!adExists)
                return _responseService.ErrorResponse<double>("Ad not found");

            // 2) Compute average in DB
            var avg = await _unitOfWork.Context.Reviews
                .AsNoTracking()
                .Where(r => r.AdsId == adId)
                .Select(r => (double?)r.Rating)
                .AverageAsync();

            double result = avg.HasValue ? Math.Round(avg.Value, 1) : 0;
            return _responseService.SuccessResponse(result);
        }

        public async Task<ServiceResponse<double>> GetAverageRatingForVendorAsync(Guid vendorId)
        {
            // 1) Vendor existence
            var vendor = await _unitOfWork.Context.Vendors
                .AsNoTracking()
                .FirstOrDefaultAsync(v => v.Id == vendorId);
            if (vendor == null)
                return _responseService.ErrorResponse<double>("Vendor not found");

            // 2) Return stored rating
            return _responseService.SuccessResponse((double)vendor.Rating);
        }

        public async Task<ServiceResponse<PagedList<GetRatingDTO>>> GetRatingsForAdAsync(Guid adId, int page = 1, int pageSize = 10)
        {
            // 1) Ad existence
            bool adExists = await _unitOfWork.Context.Ads
                .AsNoTracking()
                .AnyAsync(a => a.Id == adId);
            if (!adExists)
                return _responseService.ErrorResponse<PagedList<GetRatingDTO>>("Ad not found");

            // 2) Build query
            var query = _unitOfWork.Context.Reviews
                .AsNoTracking()
                .Where(r => r.AdsId == adId)
                .Include(r => r.Buyer).ThenInclude(b => b.Profile)
                .OrderByDescending(r => r.Created);

            // 3) Project and paginate
            return await _responseService.PagedResponseAsync(
                query.Select(r => new GetRatingDTO
                {
                    Id               = r.Id,
                    Rating           = r.Rating,
                    Comment          = r.Content,
                    CreatedAt        = r.Created,
                    BuyerId          = r.BuyerId,
                    BuyerName        = $"{r.Buyer.Profile.FirstName} {r.Buyer.Profile.LastName}",
                    BuyerInstitution = r.Buyer.Institution.Name,
                    BuyerImageUrl    = r.Buyer.Profile.ProfilePictureUrl
                }),
                page,
                pageSize,
                "AdRatings"
            );
        }

        public async Task<ServiceResponse<PagedList<GetRatingDTO>>> GetRatingsForVendorAsync(Guid vendorId, int page = 1, int pageSize = 10)
        {
            // 1) Vendor existence
            bool vendorExists = await _unitOfWork.Context.Vendors
                .AsNoTracking()
                .AnyAsync(v => v.Id == vendorId);
            if (!vendorExists)
                return _responseService.ErrorResponse<PagedList<GetRatingDTO>>("Vendor not found");

            // 2) Build query
            var query = _unitOfWork.Context.Reviews
                .AsNoTracking()
                .Where(r => r.VendorId == vendorId)
                .Include(r => r.Buyer).ThenInclude(b => b.Profile)
                .OrderByDescending(r => r.Created);

            // 3) Project and paginate
            return await _responseService.PagedResponseAsync(
                query.Select(r => new GetRatingDTO
                {
                    Id               = r.Id,
                    Rating           = r.Rating,
                    Comment          = r.Content,
                    CreatedAt        = r.Created,
                    BuyerId          = r.BuyerId,
                    BuyerName        = $"{r.Buyer.Profile.FirstName} {r.Buyer.Profile.LastName}",
                    BuyerInstitution = r.Buyer.Institution.Name,
                    BuyerImageUrl    = r.Buyer.Profile.ProfilePictureUrl
                }),
                page,
                pageSize,
                "VendorRatings"
            );
        }

        /// <summary>
        /// Recalculates and stores the vendor's average rating and review count.
        /// </summary>
        private async Task UpdateVendorRatingAsync(Guid vendorId)
        {
            var vendor = await _unitOfWork.Context.Vendors
                .AsNoTracking()
                .FirstOrDefaultAsync(v => v.Id == vendorId);
            if (vendor == null) return;

            var stats = await _unitOfWork.Context.Set<Review>()
                .Where(r => r.VendorId == vendorId)
                .GroupBy(r => 1)
                .Select(g => new
                {
                    Avg   = g.Average(r => r.Rating),
                    Count = g.Count()
                })
                .FirstOrDefaultAsync();

            if (stats != null)
            {
                vendor.Rating          = (decimal)Math.Round(stats.Avg, 1);
                vendor.NumberOfReviews = stats.Count;
                await _unitOfWork.CommitAsync();
            }
        }
    }
}

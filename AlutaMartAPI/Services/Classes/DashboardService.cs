using Microsoft.EntityFrameworkCore;
using AlutaMartAPI.Database;
using AlutaMartAPI.ModelObjects;
using AlutaMartAPI.Utilities;

namespace AlutaMartAPI.Services
{
    public class DashboardService(IUnitOfWork _unitOfWork, IResponseService _responseService) : IDashboardService
    {
        public async Task<ServiceResponse<AdminDashboardDTO>> GetAnalyticsAsync()
        {
            var profiles = await _unitOfWork.Context.Profiles
                .AsNoTracking()
                .ToListAsync();

            var roles = new[] { Roles.SuperAdmin, Roles.BusinessManager, Roles.PlatformManager, Roles.AdminUser };
            var totalAdmins = profiles.Count(p => roles.Contains(p.Role));
            var activeAdmins = profiles.Count(p => roles.Contains(p.Role) && p.IsActive);
            var inActiveAdmins = totalAdmins - activeAdmins;

            var totalVendors = profiles.Count(p => p.Role == Roles.Vendor);
            // var activeExperts = profiles.Count(p => p.Role == Roles.Expert && p.IsActive);
            // var inActiveExperts = totalExperts - activeExperts;

            var totalBuyers = profiles.Count(p => p.Role == Roles.Buyer);
            // var activeBuyers = profiles.Count(p => p.Role == Roles.Buyer && p.IsActive);
            // var inActiveBuyers = totalBuyers - activeBuyers;

            var totalSignedUpUsers = profiles.Count;

            var ads = await _unitOfWork.Context.Ads
                .AsNoTracking()
                .Select(x => new { x.Created, x.Discount, x.QuantityInStock, x.IsFeatured })
                .ToListAsync();
            var discountedAds = ads.Count(x => x.Discount == Discount.Discounted);
            var activeAds = ads.Count(x => x.QuantityInStock > 0);

            var adPurchased = await _unitOfWork.Context.PurchasedAds
                .AsNoTracking()
                .GroupBy(x => x.ProfileId)
                .Select(g => new { g.Key, adPurchasedCount = g.Count(), Created = g.First().Created })
                .ToListAsync();

            var totalAds = ads.Count;

            var popularAds = adPurchased
                .OrderByDescending(x => x.adPurchasedCount)
                .Take(10)
                .Count();

            var bottomRankedAds = adPurchased
                .OrderBy(x => x.adPurchasedCount)
                .Take(10)
                .Count();
                
            // Ads conversion rate (total)
            var totalAdsConversionRate = totalSignedUpUsers > 0 
                ? Math.Round((double)adPurchased.Count / totalSignedUpUsers * 100, 1) 
                : 0;

            var featuredAdsCount = ads.Where(x => x.IsFeatured == true).Count();

            
            // Monthly overview data
            var months = Enumerable.Range(1, 12)
                .Select(month => new DateTime(DateTime.Now.Year, month, 1))
                .ToList();

            var adsOverView = months.Select(month =>
            {
                var monthlyPurchase = adPurchased
                    .Where(x => x.Created.Year == month.Year && x.Created.Month == month.Month)
                    .ToList();

                var totalMonthlyAds = ads
                    .Count(x => x.Created.Year == month.Year && x.Created.Month == month.Month);

                var monthlySignedUpUsers = profiles
                    .Count(x => x.Created.Year == month.Year && x.Created.Month == month.Month);

                var monthlyAds = monthlyPurchase
                    .GroupBy(x => x.Key)
                    .Select(g => new { g.Key, PurchasedCount = g.Count() })
                    .ToList();

                var monthlyPopularAds = monthlyAds.OrderByDescending(x => x.PurchasedCount).Take(10).Count();
                var monthlyBottomRankedAds = monthlyAds.OrderBy(x => x.PurchasedCount).Take(10).Count();

                // Ads conversion rate (monthly)
                var monthlyAdsConversionRate = monthlySignedUpUsers > 0 
                    ? Math.Round((double)monthlyAds.Count / monthlySignedUpUsers * 100, 1) 
                    : 0;

                return new DashboardAdsOverViewDTO
                {
                    Month = month.ToString("MMMM"),
                    TotalAds = totalMonthlyAds,
                    PopularAds = monthlyPopularAds,
                    BottomRankedAds = monthlyBottomRankedAds,
                    AdsConversionRate = monthlyAdsConversionRate
                };
            }).ToList();

            var adminDashboard = new AdminDashboardDTO
            {
                TotalAdmins = totalAdmins,
                ActiveAdmins = 0,
                InActiveAdmins = 0,

                TotalVendors = totalVendors,
                ActiveVendors = 0,
                InActiveVendors = 0,

                TotalBuyers = totalBuyers,
                ActiveBuyers = 0,
                InActiveBuyers = 0,

                TotalAds = totalAdmins,
                PopularAds = popularAds,
                FeaturedAds = featuredAdsCount,
                DiscountedAds = discountedAds,
                BottomRankedAds = bottomRankedAds,
                AdsConversionRate = totalAdsConversionRate,

                AdsOverView = adsOverView
            };
            return _responseService.SuccessResponse(adminDashboard, "AdminDashboard");
        }
    }
}

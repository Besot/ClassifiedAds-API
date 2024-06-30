using AlutaMartAPI.Models;

namespace AlutaMartAPI.Database;
    public class DataSeed
    {
        public static IEnumerable<Currency> GetCurrencies()
        {
            return
                [
                    new() { Name = "us dollars", Code = "usd" },
                    new() { Name = "britain pounds", Code = "gbp" },
                    new() { Name = "naira", Code = "ngn" },
                ];
        }
        public static IEnumerable<VendorInstitution> GetVendorInstitutions()
        {
            return
                [
                    new() { Name = "AAU" },
                    new() { Name = "AAUA" },
                    new() { Name = "ABU" },
                    new() { Name = "ABUAD" },
                    new() { Name = "FUTA" },
                    new() { Name = "ELIZADE" },
                    new() { Name = "WESLEY" }
                ];
        }

        public static IEnumerable<AdsCategory> GetAdsCategories()
    {
        return
            [
                new() { Name = "phones" },
                new() { Name = "electronics" },
                new() { Name = "services" },
                new() { Name = "clothing" },
                new() { Name = "cosmetics" },
                new() { Name = "Vehicles" },
                new() { Name = "Books" },
                new() { Name = "jewelries" },
                new() { Name = "data" },
                new() { Name = "food and snacks" },
                new() { Name = "arts and creativity" },
                new() { Name = "kitchen utensils" },
                new() { Name = "laptops" },
                new() { Name = "furnitures" },
                new() { Name = "travel and culture" },
                new() { Name = "events" },
            ];
    }

    public static IEnumerable<PlanTier> GetPlanTiers()
    {
        return
            [
                new() { Name = "free tier", Amount = 0, MaxAds = 2, MaxPicture = 1, IsFeatured = false},
                new() { Name = "basic tier", Amount = 500, MaxAds = 6, MaxPicture = 2, IsFeatured = true  },
                new() { Name = "standard tier", Amount = 1000, MaxAds = 13, MaxPicture = 3, IsFeatured = true  },
                new() { Name = "premium tier", Amount = 1999, MaxAds = 20, MaxPicture = 4, IsFeatured = true  }
            ];
    }
    }

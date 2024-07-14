using System.Globalization;
using AlutaMartAPI.Models;
using AlutaMartAPI.Utilities;
using CsvHelper;
using Microsoft.AspNetCore.Identity;

namespace AlutaMartAPI.Database;
    public class DataSeed
    {
        private static IWebHostEnvironment _webHostEnv;

        public DataSeed(IWebHostEnvironment webHostEnv)
        {
            _webHostEnv = webHostEnv;
        }

        public static IEnumerable<Currency> GetCurrencies()
        {
            return
                [
                    new() { Name = "naira", Code = "ngn" },
                ];
        }


        public static IEnumerable<Institution> GetInstitutions()
        {
            var records = new List<Institution>();
            var fullPath = Path.Combine(_webHostEnv.WebRootPath, "institutions.csv");
            using (var reader = new StreamReader(fullPath))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                records = csv.GetRecords<Institution>().ToList();
            }

            return records;
        }

        public static IEnumerable<AdsCategory> GetAdsCategories()
        {
            var records = new List<AdsCategory>();
            var fullPath = Path.Combine(_webHostEnv.WebRootPath, "categories_brands.csv");
            using (var reader = new StreamReader(fullPath))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                records = csv.GetRecords<AdsCategory>().ToList();
            }

            return records;
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

        public static async Task<IEnumerable<Profile>> GetSuperAdmin(UserManager<Profile> userManager)
        {
            var superAdmin = new Profile
            {
                FirstName = Constants.AdminFirstName,
                LastName = Constants.AdminLastName,
                Email = Constants.AdminEmail,
                Role = Roles.SuperAdmin
            };

            var result = await userManager.CreateAsync(superAdmin);

            if (result.Succeeded)
            {
                result = await userManager.AddPasswordAsync(superAdmin, Constants.AdminPassword);
                if (result.Succeeded)
                {
                    return new List<Profile> { superAdmin };
                }
            }

            // Handle error (e.g., log the error or throw an exception)
            throw new Exception("Failed to create or set password for the super admin.");
        }
    }

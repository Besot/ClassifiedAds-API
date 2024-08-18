using System.Globalization;
using AlutaMartAPI.Models;
using CsvHelper;
using CsvHelper.Configuration;

namespace AlutaMartAPI.Database
{
    public class DataSeed(IWebHostEnvironment webHostEnvironment)
    {
        private readonly IWebHostEnvironment _webHostEnv = webHostEnvironment ?? throw new ArgumentNullException(nameof(webHostEnvironment));

        public IEnumerable<Currency> GetCurrencies()
        {
            return
            [
                new Currency { Name = "naira", Code = "ngn" }
            ];
        }

        public IEnumerable<Institution> GetInstitutions()
        {
            return GetRecords<Institution>("institutions.csv");
        }

        public IEnumerable<AdsCategory> GetAdsCategories()
        {
            return GetRecords<AdsCategory>("categories_brands.csv");
        }

        public IEnumerable<PlanTier> GetPlanTiers()
        {
            return
            [
                new PlanTier { Name = "free tier", Amount = 0, MaxAds = 2, MaxPicture = 1, MaxFeatured = 0 },
                new PlanTier { Name = "basic tier", Amount = 500, MaxAds = 6, MaxPicture = 2, MaxFeatured = 1 },
                new PlanTier { Name = "standard tier", Amount = 1000, MaxAds = 13, MaxPicture = 3, MaxFeatured = 4 },
                new PlanTier { Name = "premium tier", Amount = 1999, MaxAds = 20, MaxPicture = 4, MaxFeatured = 8 },
            ];
        }

        private List<T> GetRecords<T>(string fileName)
        {
            var fullPath = Path.Combine(_webHostEnv.WebRootPath, fileName);

            if (!File.Exists(fullPath))
            {
                throw new FileNotFoundException("The CSV file was not found.", fullPath);
            }

            var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HeaderValidated = null,
                MissingFieldFound = null,
                IgnoreBlankLines = true,
                BadDataFound = null,
                HasHeaderRecord = true
            };

            using var reader = new StreamReader(fullPath);
            using var csv = new CsvReader(reader, csvConfig);
            return csv.GetRecords<T>().ToList();
        }
    }
}

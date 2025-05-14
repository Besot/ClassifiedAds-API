using AlutaMartAPI.Utilities;

namespace AlutaMartAPI.DTOs;
    public class GetAdsDTO
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string BrandName { get; set; }
        public int QuantityInStock { get; set; }
        public string VendorImage { get; set; }
        public double Price { get; set; }
        public double? DiscountPrice { get; set; }
        public string AdsImageUrl { get; set; }
        public DateTimeOffset? ExpiryDate { get; set; }
        public string Status { get; set; }
        public bool IsFeatured { get; set; } = false;
        public AdsType AdsType { get; set; }
        public Discount Discount { get; set; }
        public string AdsCondition { get; set; }
        public long NumberOfReviews { get; set; }
        public Guid VendorId { get; set; }
        public Guid CurrencyId { get; set; }
        public List<CategoryDTO> Categories { get; set; }
    }

    public class CategoryDTO
    {
        public string Name { get; set; }
         public Guid Id { get; internal set; }
}
using AlutaMartAPI.Utilities;

namespace AlutaMartAPI.DTOs;
    public class GetAdDetailsDTO
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string BrandName { get; set; }
        public string VendorImage { get; set; }
        public double Price { get; set; }
        public int QuantityInStock { get; set; }
        public double DiscountPrice { get; set; }
        public List<string> AdsImageUrl { get; set; }
        public DateTimeOffset? ExpiryDate { get; set; }
        public AdsStatus Status { get; set; }
        public bool IsFeatured { get; set; } = false;
        public AdsType AdsType { get; set; }
        public Discount Discount { get; set; }
        public AdsCondition AdsCondition { get; set; }
        public long NumberOfReviews { get; set; }
        public Guid VendorId { get; set; }
        public string AdCategory { get; set; }
        public Guid AdCategoryId { get; set; }
        public Guid CurrencyId { get; set; }
        public List<GetVendorReviewDTO> Reviews { get; set; }

    }
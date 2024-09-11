using AlutaMartAPI.Utilities;

namespace AlutaMartAPI.Models;
public class Ads :BaseEntity
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public int QuantityInStock { get; set; }
        public double Price { get; set; }
        public double? DiscountPrice { get; set; }
        public DateTimeOffset? FeaturedExpiryDate { get; set; }

        public AdsStatus Status { get; set; } = AdsStatus.Active;

        public bool IsFeatured { get; set; } = false;

        public AdsType AdsType { get; set; }
        public Discount Discount { get; set; }
        public AdsCondition AdsCondition { get; set; }
        public long NumberOfReviews { get; set; }
        public Guid VendorId { get; set; }
        public virtual Vendor Vendor { get; set; }

        public Guid AdsCategoryId { get; set; }
        public virtual AdsCategory AdsCategory { get; set; }

        public Guid CurrencyId { get; set; }
        public virtual Currency Currency { get; set; }
    }
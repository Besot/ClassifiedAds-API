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
        public Guid CurrencyId { get; set; }
        public virtual Currency Currency { get; set; }
         public double Latitude { get; set; }
        public double Longitude { get; set; }
        public virtual ICollection<AdsImage> AdsImages { get; set; } = new List<AdsImage>();
        public virtual ICollection<AdsCategory> AdsCategories { get; set; } = new List<AdsCategory>();
        public virtual ICollection<Review> AdsReviews { get; set; } = new List<Review>();
        public virtual ICollection<Report> AdReports { get; set; } = new List<Report>();
    }


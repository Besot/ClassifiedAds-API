namespace AlutaMartAPI.Models;
    public class Cart : BaseEntity
    {
        public Guid AdsId { get; set; }
        public virtual Ads Ads { get; set; }

        public Guid VendorId { get; set; }
        public virtual Vendor Vendor { get; set; }

        public Guid ProfileId { get; set; }
        public virtual Profile Profile { get; set; }

        public Guid? BuyerId { get; set; }
        public virtual Buyer Buyer { get; set; }
        public int QuantityAdded { get; set; } // Number of ads added to the cart
        public double PriceAtTimeOfAdd { get; set; } // Save price to prevent price manipulation later

    }

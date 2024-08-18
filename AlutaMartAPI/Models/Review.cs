namespace AlutaMartAPI.Models;
    public class Review :BaseEntity
    {
        public string Content { get; set; }
        public Guid? VendorId { get; set; }
        public virtual Vendor Vendor { get; set; }
        public Guid? AdsId { get; set; }
        public virtual Ads Ads { get; set; }
        public Guid BuyerId { get; set; }
        public virtual Buyer Buyer { get; set; }
    }
namespace AlutaMartAPI.Models;
    public class VendorReview :BaseEntity
    {
        public string Review { get; set; }
        public Guid VendorId { get; set; }
        public virtual Vendor Vendor { get; set; }
        public Guid BuyerId { get; set; }
        public virtual Buyer Buyer { get; set; }
    }
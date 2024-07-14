namespace AlutaMartAPI.Models;
    public class AdsComment :BaseEntity
    {
        public string Comment { get; set; }
        public Guid AdsId { get; set; }
        public virtual Ads Ads { get; set; }
        public Guid BuyerId { get; set; }
        public virtual Buyer Buyer { get; set; }
    }
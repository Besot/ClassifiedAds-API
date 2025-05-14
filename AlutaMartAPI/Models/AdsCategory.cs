namespace AlutaMartAPI.Models;
    public class Category : BaseEntity
    {
        public string Name { get; set; }
        public string Brand { get; set; }
        public string Code { get; set; }

    }

    public class AdsCategory : BaseEntity
    {
        public Guid AdsId { get; set; }
        public virtual Ads Ads { get; set; }

        public Guid CategoryId { get; set; }
        public virtual Category Category { get; set; }   
    }
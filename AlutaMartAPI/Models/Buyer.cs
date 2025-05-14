namespace AlutaMartAPI.Models;
    public class Buyer : BaseEntity
    {
        public Guid ProfileId { get; set; }
        public virtual Profile Profile { get; set; }
        public Guid? InstitutionId { get; set; }
        public virtual Institution Institution { get; set; }
        public int AdPurchasedCount { get; set; }
    }

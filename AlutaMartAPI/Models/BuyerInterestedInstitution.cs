namespace AlutaMartAPI.Models;
    public class BuyerInterestedInstitution : BaseEntity
    {
        public Guid ProfileId { get; set; }
        public virtual Profile Profile {get; set; }

        public Guid BuyerId { get; set; }
        public virtual Buyer Buyer { get; set; }
        
        public Guid InstitutionId { get; set; }
        public virtual Institution Institution { get; set; }
    }
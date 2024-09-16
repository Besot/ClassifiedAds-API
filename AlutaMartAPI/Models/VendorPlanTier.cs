namespace AlutaMartAPI.Models;
    public class VendorPlanTier : BaseEntity
    {
        public Guid ProfileId { get; set; }
        public virtual Profile Profile {get; set; }

        public Guid VendorId { get; set; }
        public virtual Vendor Vendor { get; set; }
        
        public Guid? PlanTierId { get; set; }
        public virtual PlanTier PlanTier { get; set; }
        public DateTimeOffset? ExpiryDate { get; set; }

    }
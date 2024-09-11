
using AlutaMartAPI.Utilities;

namespace AlutaMartAPI.Models;

    public class Vendor : BaseEntity
    {
        public string BrandName { get; set; }
        public string Bio { get; set; } 
        public string InstaUrl { get; set; }
        public string XUrl { get; set; }
        public string FacebookUrl { get; set; }
        public string VendorPictureUrl { get; set; }
        public Guid InstitutionId { get; set; }
        public virtual Institution VendorInstitution { get; set; }
        public Guid ProfileId { get; set; }
        public virtual Profile Profile { get; set; }
        public decimal Rating { get; set; }
        public long NumberOfReviews { get; set; }
        public VerificationStatus VerificationStatus { get; set; }
        public IdentityCard NIN { get; set; }
    }

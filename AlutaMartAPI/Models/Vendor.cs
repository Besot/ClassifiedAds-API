
using AlutaMartAPI.Utilities;

namespace AlutaMartAPI.Models;

    public class Vendor : BaseEntity
    {
        public string Department { get; set; }

        public string Bio { get; set; } 
        public string InstaUrl { get; set; }
        public string XUrl { get; set; }
        public string FacebookUrl { get; set; }
        public string ProfilePictureUrl { get; set; }
        public Guid VendorInstitutionId { get; set; }
        public virtual VendorInstitution VendorInstitution { get; set; }
        public Guid ProfileId { get; set; }
        public virtual Profile Profile { get; set; }
        public decimal Rating { get; set; }
        public long NumberOfReviews { get; set; }
        public VerificationStatus VerificationStatus { get; set; }
        public Level AcademicLevel { get; set; }
    }

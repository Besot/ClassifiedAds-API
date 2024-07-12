
using AlutaMartAPI.Utilities;

namespace AlutaMartAPI.Models;

    public class Vendor : BaseEntity
    {
        public string Experience { get; set; }

        public string Bio { get; set; } 
        public string InstaUrl { get; set; }
        public string TwitterUrl { get; set; }
        public string FacebookUrl { get; set; }

        public string ProfilePictureUrl { get; set; }
        public ApprovalStatus VendorInfoStatus { get; set; }

        public Guid VendorInstitutionId { get; set; }
        public virtual VendorInstitution VendorInstitution { get; set; }

        public Guid ProfileId { get; set; }
        public virtual Profile Profile { get; set; }
        public decimal Rating { get; set; }
        public int NumberOfReviews { get; set; }
    }

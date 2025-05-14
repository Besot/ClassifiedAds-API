using AlutaMartAPI.Models;
using AlutaMartAPI.Utilities;

namespace AlutaMartAPI.DTOs;
    public class GetVendorDTO
    {
        public Guid Id { get; set; }
        public string BrandName { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public Roles Role { get; set; }
        public VerificationStatus VerificationStatus { get; set; }
        public string Department { get; set; }
        public string Bio { get; set; }   
        public string InstaUrl { get; set; }
        public string XUrl { get; set; }
        public string FacebookUrl { get; set; }
        public string ProfilePictureUrl { get; set; }
        public Guid? VendorInstitutionId { get; set; }
        public IdentityCard NIN { get; set; }
        public long TotalAds { get; set; }
        public long NumberOfReviews { get; set; }
        public decimal Rating { get; set; }
        public List<GetVendorReviewDTO> VendorReview { get; set; }

    }
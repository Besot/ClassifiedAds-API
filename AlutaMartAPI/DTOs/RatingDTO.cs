using System;
using System.ComponentModel.DataAnnotations;

namespace AlutaMartAPI.DTOs
{
    public class CreateRatingDTO
    {
        [Required]
        public Guid TargetId { get; set; } // Either ad ID or vendor ID
        
        [Required]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
        public int Rating { get; set; }
        
        public string Comment { get; set; }
        
        public bool IsVendorRating { get; set; } // true for vendor rating, false for ad rating
    }

    public class GetRatingDTO
    {
        public Guid Id { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        
        public Guid BuyerId { get; set; }
        public string BuyerName { get; set; }
        public string BuyerInstitution { get; set; }
        public string BuyerImageUrl { get; set; }
    }
}

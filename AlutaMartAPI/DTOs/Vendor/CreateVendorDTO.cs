using System.ComponentModel.DataAnnotations;
using AlutaMartAPI.Utilities;

namespace AlutaMartAPI.DTOs;
    public class CreateVendorDTO
    {
        [Required(ErrorMessage ="vendor's brand name is required")]
        [MaxLength(100, ErrorMessage ="vendor's brand name should not be more than 100 characters")]
	    [MinLength(5, ErrorMessage ="vendor's brand name should not be less than 3 characters")]
        public string BrandName { get; set; }

        [Required(ErrorMessage ="vendor's bio is required")]
        [MaxLength(400, ErrorMessage ="vendor's bio should not be more than 400 characters")]
	    [MinLength(5, ErrorMessage ="vendor's bio should not be less than 5 characters")]
        public string Bio { get; set; }

        [MaxLength(100, ErrorMessage ="Vendor's istagramUrl link should not be more than 100 characters")]
	    [MinLength(20, ErrorMessage ="Vendor's istagramUrl link should not be less than 10 characters")]
        public string InstaUrl { get; set; }

        [MaxLength(100, ErrorMessage ="Vendor's xUrl link should not be more than 100 characters")]
	    [MinLength(20, ErrorMessage ="Vendor's xUrl link should not be less than 10 characters")]
        public string XUrl { get; set; }

        [MaxLength(100, ErrorMessage ="Vendor's facebookUrl link should not be more than 100 characters")]
	    [MinLength(20, ErrorMessage ="Vendor's facebookUrl link should not be less than 10 characters")]
        public string FacebookUrl { get; set; }

        [Required(ErrorMessage ="Vendor's ProfileUrl link is required")]
        [MaxLength(100, ErrorMessage ="Vendor's ProfileUrl link should not be more than 100 characters")]
	    [MinLength(20, ErrorMessage ="Vendor's ProfileUrl link should not be less than 20 characters")]
        public string ProfilePictureUrl { get; set; }

        public Guid? InstitutionId { get; set; }

        [Required(ErrorMessage ="NIN is required")]
        [MaxLength(11, ErrorMessage ="NIN should not be more than 11 characters")]
	    [MinLength(11, ErrorMessage ="NIN should not be less than 11 characters")]        
        public string NIN { get; set; }
    }
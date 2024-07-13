using System.ComponentModel.DataAnnotations;
using AlutaMartAPI.Utilities;

namespace AlutaMartAPI.DTOs;
    public class CreateVendorDTO
    {
        [Required(ErrorMessage ="vendor's department is required")]
        [MaxLength(100, ErrorMessage ="vendor's department should not be more than 100 characters")]
	    [MinLength(5, ErrorMessage ="vendor's department should not be less than 5 characters")]
        public string Department { get; set; }

        [Required(ErrorMessage ="vendor's bio is required")]
        [MaxLength(400, ErrorMessage ="vendor's bio should not be more than 400 characters")]
	    [MinLength(5, ErrorMessage ="vendor's bio should not be less than 5 characters")]
        public string Bio { get; set; }
            
        public string InstaUrl { get; set; }
        public string XUrl { get; set; }
        public string FacebookUrl { get; set; }

        [Required(ErrorMessage ="Vendor's ProfileUrl link is required")]
        [MaxLength(100, ErrorMessage ="Vendor's ProfileUrl link should not be more than 100 characters")]
	    [MinLength(20, ErrorMessage ="Vendor's ProfileUrl link should not be less than 20 characters")]
        public string ProfilePictureUrl { get; set; }

        public Guid? VendorInstitutionId { get; set; }

        [Required(ErrorMessage ="Academic Level is required")]
        [Range(1, 6, ErrorMessage ="Level value should be between 1 and 6")]
        public Level AcademicLevel { get; set; }
    }
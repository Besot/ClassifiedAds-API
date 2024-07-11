using System.ComponentModel.DataAnnotations;
using AlutaMartAPI.Utilities;

namespace AlutaMartAPI.DTOs;
    public class CreateAdminDTO
    {
	[Required(ErrorMessage ="first name is required")]
	[AlphaNumeric(ErrorMessage ="first name should be alphanumeric characters only")]
    [MaxLength(50, ErrorMessage ="first name should not be more than 50 characters")]
    [MinLength(3, ErrorMessage ="first name should not be less than 3 characters")]
	public string FirstName { get; set; }

	[Required(ErrorMessage ="last name is required")]
	[AlphaNumeric(ErrorMessage ="last name should be alphanumeric characters only")]
    [MaxLength(50, ErrorMessage ="last name should not be more than 50 characters")]
    [MinLength(3, ErrorMessage ="last name should not be less than 3 characters")]
	public string LastName { get; set; }

	[Required(ErrorMessage ="email is required")]
	[MaxLength(60, ErrorMessage ="email should not be more than 60 characters")]
	[MinLength(4, ErrorMessage ="email should not be less than 4 characters")]
	public string Email { get; set; }
    
    [Range(4, 6, ErrorMessage ="roles value should be between 4 and 6")]
    public Roles AdminRole { get; set; }
}
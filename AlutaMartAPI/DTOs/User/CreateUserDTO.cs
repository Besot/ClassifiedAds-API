using System.ComponentModel.DataAnnotations;
using AlutaMartAPI.Utilities;

namespace AlutaMartAPI.DTOs;

public class CreateUserDTO
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

	[Required(ErrorMessage ="Password is required")]
	[MaxLength(60, ErrorMessage ="Password should not be more than 60 characters")]
	[MinLength(7, ErrorMessage ="Password should not be less than 7 characters")]
	public string Password { get; set; }
}
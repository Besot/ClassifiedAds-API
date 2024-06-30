using System.ComponentModel.DataAnnotations;

namespace AlutaMartAPI.DTOs;

public class ResetPasswordDTO
{
	[Required(ErrorMessage ="Email is required")]
	[MaxLength(60, ErrorMessage ="email should not be more than 60 characters")]
	[MinLength(4, ErrorMessage ="email should not be less than 4 characters")]
	public string Email { get; set; }
}
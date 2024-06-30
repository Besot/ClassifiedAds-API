using System.ComponentModel.DataAnnotations;

namespace AlutaMartAPI.DTOs;

public class CreatePasswordDTO
{
    [Range(100000, 999999, ErrorMessage ="Password reset code is invalid")]
    public long Token { get; set; }

    [Required(ErrorMessage ="Password is required")]
    [MaxLength(50, ErrorMessage ="Password should not be more than 50 characters")]
    [MinLength(7, ErrorMessage ="Password should not be less than 7 characters")]
    public string Password { get; set; }

    public string ConfirmPassword { get; set; }
}

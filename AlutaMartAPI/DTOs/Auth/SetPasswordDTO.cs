using System.ComponentModel.DataAnnotations;

namespace AlutaMartAPI.DTOs;
    public class SetPasswordDTO
    {
    [Required(ErrorMessage ="Password is required")]
    [MaxLength(50, ErrorMessage ="Password should not be more than 50 characters")]
    [MinLength(7, ErrorMessage ="Password should not be less than 7 characters")]
    public string Password { get; set; }

    [Required(ErrorMessage ="Confirm Password is required")]
    [MaxLength(50, ErrorMessage ="Confirm Password should not be more than 50 characters")]
    [MinLength(7, ErrorMessage ="Confirm Password should not be less than 7 characters")]
    public string ConfirmPassword { get; set; }
    }
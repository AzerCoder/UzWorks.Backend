using System.ComponentModel.DataAnnotations;

namespace UzWorks.Core.DataTransferObjects.Auth;

public class LoginDto
{
    [Required(ErrorMessage = "Phone number is required.")]
    [RegularExpression("^998\\d{9}$", ErrorMessage = "Please enter a valid phone number starting with 998 and 12 digits long.")]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required(ErrorMessage ="Password is required.")]
    [MinLength(8)]
    [MaxLength(30)]
    public string Password { get; set; } = string.Empty;
}

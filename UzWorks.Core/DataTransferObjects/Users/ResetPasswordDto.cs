using System.ComponentModel.DataAnnotations;

namespace UzWorks.Core.DataTransferObjects.Users;

public class ResetPasswordDto
{
    [Required]
    public Guid UserId { get; set; }
    
    [Required]
    public string NewPassword { get; set; } = string.Empty;
    
    [Required]
    public string ConfirmPassword { get; set; } = string.Empty;

    [Required]
    public string OldPassword { get; set; } = string.Empty;
}

using System.ComponentModel.DataAnnotations;

namespace UzWorks.Core.DataTransferObjects.Experiences;

public class ExperienceDto
{
    [Required]
    public string CompanyName { get; set; } = string.Empty;

    [Required]
    public string Position { get; set; } = string.Empty;

    [Required]
    public DateTime StartDate { get; set; }

    [Required]
    public DateTime EndDate { get; set; }

    public string Description { get; set; } = string.Empty;
}

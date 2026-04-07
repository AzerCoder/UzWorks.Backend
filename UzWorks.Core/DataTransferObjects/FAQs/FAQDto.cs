using System.ComponentModel.DataAnnotations;

namespace UzWorks.Core.DataTransferObjects.FAQs;

public class FAQDto
{
    [Required]
    public string Question { get; set; } = string.Empty;
    
    [Required]
    public string Answer { get; set; } = string.Empty;
}

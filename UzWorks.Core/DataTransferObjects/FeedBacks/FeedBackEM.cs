using System.ComponentModel.DataAnnotations;

namespace UzWorks.Core.DataTransferObjects.FeedBacks;

public class FeedBackEM
{
    [Required]
    public Guid Id { get; set; }
    
    [Required]
    public string Message { get; set; } = string.Empty;
    
    [Required]
    public string FullName { get; set; } = string.Empty;

    [Required]
    public DateTime DueDate { get; set; }
}

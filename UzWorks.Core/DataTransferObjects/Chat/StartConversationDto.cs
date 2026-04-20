using System.ComponentModel.DataAnnotations;

namespace UzWorks.Core.DataTransferObjects.Chat;

public class StartConversationDto
{
    [Required]
    public Guid OtherUserId { get; set; }
    public Guid? JobId { get; set; }
    public Guid? WorkerId { get; set; }
}

using System.ComponentModel.DataAnnotations;

namespace UzWorks.Core.DataTransferObjects.Chat;

public class SendMessageDto
{
    [Required]
    public Guid ConversationId { get; set; }

    [Required]
    [MaxLength(2000)]
    public string Content { get; set; } = string.Empty;
}

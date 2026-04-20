namespace UzWorks.Core.DataTransferObjects.Chat;

public class MessageVM
{
    public Guid Id { get; set; }
    public Guid ConversationId { get; set; }
    public Guid SenderId { get; set; }
    public string Content { get; set; } = string.Empty;
    public bool IsRead { get; set; }
    public DateTime? SentAt { get; set; }
}

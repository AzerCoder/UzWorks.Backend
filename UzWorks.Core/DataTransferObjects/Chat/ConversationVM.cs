namespace UzWorks.Core.DataTransferObjects.Chat;

public class ConversationVM
{
    public Guid Id { get; set; }
    public Guid OtherUserId { get; set; }
    public string OtherUserFullName { get; set; } = string.Empty;
    public Guid? JobId { get; set; }
    public Guid? WorkerId { get; set; }
    public MessageVM? LastMessage { get; set; }
    public int UnreadCount { get; set; }
    public DateTime? CreatedAt { get; set; }
}

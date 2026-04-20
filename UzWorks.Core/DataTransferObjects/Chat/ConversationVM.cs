namespace UzWorks.Core.DataTransferObjects.Chat;

public class ConversationVM
{
    public Guid Id { get; set; }
    public Guid ParticipantOneId { get; set; }
    public Guid ParticipantTwoId { get; set; }
    public Guid? JobId { get; set; }
    public Guid? WorkerId { get; set; }
    public DateTime? CreateDate { get; set; }
    public MessageVM? LastMessage { get; set; }
    public int UnreadCount { get; set; }
}

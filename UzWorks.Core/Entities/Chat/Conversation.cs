namespace UzWorks.Core.Entities.Chat;

public class Conversation : BaseEntity
{
    public Guid ParticipantOneId { get; set; }
    public Guid ParticipantTwoId { get; set; }
    public Guid? JobId { get; set; }
    public Guid? WorkerId { get; set; }
    public List<Message>? Messages { get; set; }
}

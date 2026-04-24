namespace UzWorks.Core.Entities.Chat;

public class Conversation : BaseEntity
{
    public Guid ParticipantOneId { get; set; }
    public Guid ParticipantTwoId { get; set; }
    public Guid? JobId { get; set; }
    public Guid? WorkerId { get; set; }

    /// <summary>True when ParticipantOne has deleted this conversation on their side.</summary>
    public bool IsDeletedByParticipantOne { get; set; } = false;

    /// <summary>True when ParticipantTwo has deleted this conversation on their side.</summary>
    public bool IsDeletedByParticipantTwo { get; set; } = false;

    public List<Message>? Messages { get; set; }
}

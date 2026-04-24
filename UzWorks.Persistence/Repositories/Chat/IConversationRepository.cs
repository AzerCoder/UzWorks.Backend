using UzWorks.Core.Entities.Chat;

namespace UzWorks.Persistence.Repositories.Chat;

public interface IConversationRepository : IGenericRepository<Conversation>
{
    Task<Conversation?> GetByParticipantsAsync(Guid userOneId, Guid userTwoId, Guid? jobId, Guid? workerId);
    Task<Conversation[]> GetByUserIdAsync(Guid userId);
    Task<Conversation?> GetWithMessagesAsync(Guid conversationId);

    /// <summary>
    /// Marks the conversation as deleted for the given user.
    /// Returns true if BOTH sides have now deleted → caller should hard-delete.
    /// </summary>
    Task<bool> SoftDeleteForUserAsync(Guid conversationId, Guid userId);
}

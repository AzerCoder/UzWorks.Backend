using UzWorks.Core.Entities.Chat;

namespace UzWorks.Persistence.Repositories.Chat;

public interface IConversationRepository : IGenericRepository<Conversation>
{
    Task<Conversation?> GetByParticipantsAsync(Guid userOneId, Guid userTwoId, Guid? jobId, Guid? workerId);
    Task<Conversation[]> GetByUserIdAsync(Guid userId);
    Task<Conversation?> GetWithMessagesAsync(Guid conversationId);
}

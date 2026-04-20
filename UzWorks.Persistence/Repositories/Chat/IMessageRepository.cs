using UzWorks.Core.Entities.Chat;

namespace UzWorks.Persistence.Repositories.Chat;

public interface IMessageRepository : IGenericRepository<Message>
{
    Task<Message[]> GetByConversationIdAsync(Guid conversationId, int pageNumber, int pageSize);
    Task MarkAsReadAsync(Guid conversationId, Guid userId);
    Task<int> GetUnreadCountAsync(Guid conversationId, Guid userId);
}

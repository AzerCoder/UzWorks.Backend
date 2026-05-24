using UzWorks.Core.Entities.Chat;

namespace UzWorks.Persistence.Repositories.Chat;

public interface IMessageRepository : IGenericRepository<Message>
{
    Task<Message[]> GetByConversationIdAsync(Guid conversationId, int pageNumber, int pageSize);

    /// <summary>
    /// Marks unread messages as read and returns the distinct sender IDs
    /// whose messages were marked (used to send read-receipt notifications).
    /// </summary>
    Task<IReadOnlyList<Guid>> MarkAsReadAsync(Guid conversationId, Guid userId);

    Task<int> GetUnreadCountAsync(Guid conversationId, Guid userId);
}

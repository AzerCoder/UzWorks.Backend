using Microsoft.EntityFrameworkCore;
using UzWorks.Core.Entities.Chat;
using UzWorks.Persistence.Data;

namespace UzWorks.Persistence.Repositories.Chat;

public class MessageRepository : GenericRepository<Message>, IMessageRepository
{
    public MessageRepository(UzWorksDbContext context) : base(context) { }

    /// <summary>
    /// Returns paginated messages in ascending order (oldest → newest).
    /// pageNumber = 1 returns the first page. Pass 0/0 to get all messages.
    /// </summary>
    public async Task<Message[]> GetByConversationIdAsync(
        Guid conversationId, int pageNumber, int pageSize)
    {
        var query = _dbSet
            .Where(m => m.ConversationId == conversationId && !m.IsDeleted)
            .OrderBy(m => m.CreateDate);   // ascending: oldest first → newest last

        if (pageNumber > 0 && pageSize > 0)
            query = (IOrderedQueryable<Message>)query
                .Skip(pageSize * (pageNumber - 1))
                .Take(pageSize);

        return await query.ToArrayAsync();
    }

    /// <summary>
    /// Marks as read all messages sent by others in this conversation.
    /// Returns the distinct sender IDs of the messages that were marked
    /// (so the Hub can fire read-receipt events to those users).
    /// </summary>
    public async Task<IReadOnlyList<Guid>> MarkAsReadAsync(Guid conversationId, Guid userId)
    {
        var unread = await _dbSet
            .Where(m => m.ConversationId == conversationId
                     && m.SenderId != userId
                     && !m.IsRead
                     && !m.IsDeleted)
            .ToListAsync();

        if (unread.Count == 0)
            return Array.Empty<Guid>();

        foreach (var msg in unread)
            msg.IsRead = true;

        await _context.SaveChangesAsync();

        return unread.Select(m => m.SenderId).Distinct().ToList();
    }

    public async Task<int> GetUnreadCountAsync(Guid conversationId, Guid userId) =>
        await _dbSet.CountAsync(m =>
            m.ConversationId == conversationId
            && m.SenderId != userId
            && !m.IsRead
            && !m.IsDeleted);
}

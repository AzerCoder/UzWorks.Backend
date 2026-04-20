using Microsoft.EntityFrameworkCore;
using UzWorks.Core.Entities.Chat;
using UzWorks.Persistence.Data;

namespace UzWorks.Persistence.Repositories.Chat;

public class MessageRepository : GenericRepository<Message>, IMessageRepository
{
    public MessageRepository(UzWorksDbContext context) : base(context) { }

    public async Task<Message[]> GetByConversationIdAsync(Guid conversationId, int pageNumber, int pageSize)
    {
        var query = _dbSet
            .Where(m => m.ConversationId == conversationId && !m.IsDeleted)
            .OrderByDescending(m => m.CreateDate);

        if (pageNumber > 0 && pageSize > 0)
            return await query.Skip(pageSize * (pageNumber - 1)).Take(pageSize).ToArrayAsync();

        return await query.ToArrayAsync();
    }

    public async Task MarkAsReadAsync(Guid conversationId, Guid userId)
    {
        var unread = await _dbSet
            .Where(m => m.ConversationId == conversationId && m.SenderId != userId && !m.IsRead)
            .ToListAsync();

        foreach (var msg in unread)
            msg.IsRead = true;

        await _context.SaveChangesAsync();
    }

    public async Task<int> GetUnreadCountAsync(Guid conversationId, Guid userId) =>
        await _dbSet.CountAsync(m => m.ConversationId == conversationId && m.SenderId != userId && !m.IsRead);
}

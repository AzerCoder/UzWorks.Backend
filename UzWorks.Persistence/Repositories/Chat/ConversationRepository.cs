using Microsoft.EntityFrameworkCore;
using UzWorks.Core.Entities.Chat;
using UzWorks.Persistence.Data;

namespace UzWorks.Persistence.Repositories.Chat;

public class ConversationRepository : GenericRepository<Conversation>, IConversationRepository
{
    public ConversationRepository(UzWorksDbContext context) : base(context) { }

    public async Task<Conversation?> GetByParticipantsAsync(Guid userOneId, Guid userTwoId, Guid? jobId, Guid? workerId) =>
        await _dbSet.FirstOrDefaultAsync(c =>
            !c.IsDeleted &&
            ((c.ParticipantOneId == userOneId && c.ParticipantTwoId == userTwoId) ||
             (c.ParticipantOneId == userTwoId && c.ParticipantTwoId == userOneId)) &&
            c.JobId == jobId &&
            c.WorkerId == workerId);

    public async Task<Conversation[]> GetByUserIdAsync(Guid userId) =>
        await _dbSet
            .Include(c => c.Messages)
            .Where(c => !c.IsDeleted &&
                (c.ParticipantOneId == userId || c.ParticipantTwoId == userId))
            .OrderByDescending(c => c.Messages!.Max(m => (DateTime?)m.CreateDate) ?? c.CreateDate)
            .ToArrayAsync();

    public async Task<Conversation?> GetWithMessagesAsync(Guid conversationId) =>
        await _dbSet
            .Include(c => c.Messages!.OrderBy(m => m.CreateDate))
            .FirstOrDefaultAsync(c => c.Id == conversationId && !c.IsDeleted);
}

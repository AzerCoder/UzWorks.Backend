using Microsoft.EntityFrameworkCore;
using UzWorks.Core.Entities.Chat;
using UzWorks.Persistence.Data;

namespace UzWorks.Persistence.Repositories.Chat;

public class ConversationRepository : GenericRepository<Conversation>, IConversationRepository
{
    public ConversationRepository(UzWorksDbContext context) : base(context) { }

    /// <summary>
    /// Finds an existing conversation between two users.
    /// Only returns conversations that have NOT been soft-deleted by the requesting user.
    /// </summary>
    public async Task<Conversation?> GetByParticipantsAsync(
        Guid userOneId, Guid userTwoId, Guid? jobId, Guid? workerId) =>
        await _dbSet.FirstOrDefaultAsync(c =>
            !c.IsDeleted &&
            (
                // userOne is ParticipantOne and hasn't deleted it
                (c.ParticipantOneId == userOneId && c.ParticipantTwoId == userTwoId && !c.IsDeletedByParticipantOne) ||
                // userOne is ParticipantTwo and hasn't deleted it
                (c.ParticipantOneId == userTwoId && c.ParticipantTwoId == userOneId && !c.IsDeletedByParticipantTwo)
            ) &&
            c.JobId == jobId &&
            c.WorkerId == workerId);

    /// <summary>
    /// Returns all conversations visible to the given user
    /// (excludes conversations soft-deleted by that user).
    /// </summary>
    public async Task<Conversation[]> GetByUserIdAsync(Guid userId) =>
        await _dbSet
            .Include(c => c.Messages)
            .Where(c => !c.IsDeleted &&
                (
                    (c.ParticipantOneId == userId && !c.IsDeletedByParticipantOne) ||
                    (c.ParticipantTwoId == userId && !c.IsDeletedByParticipantTwo)
                ))
            .OrderByDescending(c => c.Messages!.Max(m => (DateTime?)m.CreateDate) ?? c.CreateDate)
            .ToArrayAsync();

    public async Task<Conversation?> GetWithMessagesAsync(Guid conversationId) =>
        await _dbSet
            .Include(c => c.Messages!.OrderBy(m => m.CreateDate))
            .FirstOrDefaultAsync(c => c.Id == conversationId && !c.IsDeleted);

    /// <summary>
    /// Soft-deletes the conversation for the given user.
    /// Returns true when BOTH participants have deleted → the caller should hard-delete.
    /// </summary>
    public async Task<bool> SoftDeleteForUserAsync(Guid conversationId, Guid userId)
    {
        var conversation = await _dbSet.FirstOrDefaultAsync(c => c.Id == conversationId && !c.IsDeleted)
            ?? throw new InvalidOperationException($"Conversation {conversationId} not found.");

        if (conversation.ParticipantOneId == userId)
            conversation.IsDeletedByParticipantOne = true;
        else if (conversation.ParticipantTwoId == userId)
            conversation.IsDeletedByParticipantTwo = true;
        else
            throw new UnauthorizedAccessException("User is not a participant of this conversation.");

        await _context.SaveChangesAsync();

        // Both sides deleted → signal hard delete
        return conversation.IsDeletedByParticipantOne && conversation.IsDeletedByParticipantTwo;
    }
}

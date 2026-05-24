using UzWorks.Core.DataTransferObjects.Chat;

namespace UzWorks.BL.Services.Chat;

public interface IChatService
{
    Task<ConversationVM> StartOrGetConversationAsync(Guid currentUserId, StartConversationDto dto);
    Task<IEnumerable<ConversationVM>> GetUserConversationsAsync(Guid userId);
    Task<ConversationVM> GetConversationAsync(Guid conversationId, Guid userId);
    Task<MessageVM> SendMessageAsync(Guid senderId, SendMessageDto dto);
    Task<IEnumerable<MessageVM>> GetMessagesAsync(Guid conversationId, Guid userId, int pageNumber, int pageSize);
    /// <summary>
    /// Marks messages as read. Returns sender IDs whose messages were marked
    /// (used by the Hub to fire read-receipt notifications).
    /// </summary>
    Task<IReadOnlyList<Guid>> MarkAsReadAsync(Guid conversationId, Guid userId);
    Task DeleteConversationAsync(Guid conversationId, Guid userId);

    /// <summary>Returns (participantOneId, participantTwoId) for the given conversation.</summary>
    Task<(Guid P1, Guid P2)> GetParticipantIdsAsync(Guid conversationId);
}

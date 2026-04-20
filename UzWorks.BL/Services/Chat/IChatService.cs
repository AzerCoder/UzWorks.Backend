using UzWorks.Core.DataTransferObjects.Chat;

namespace UzWorks.BL.Services.Chat;

public interface IChatService
{
    Task<ConversationVM> StartOrGetConversationAsync(Guid currentUserId, StartConversationDto dto);
    Task<IEnumerable<ConversationVM>> GetUserConversationsAsync(Guid userId);
    Task<ConversationVM> GetConversationAsync(Guid conversationId, Guid userId);
    Task<MessageVM> SendMessageAsync(Guid senderId, SendMessageDto dto);
    Task<IEnumerable<MessageVM>> GetMessagesAsync(Guid conversationId, Guid userId, int pageNumber, int pageSize);
    Task MarkAsReadAsync(Guid conversationId, Guid userId);
}

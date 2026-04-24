using UzWorks.Core.Abstract;
using UzWorks.Core.DataTransferObjects.Chat;
using UzWorks.Core.Entities.Chat;
using UzWorks.Core.Exceptions;
using UzWorks.Identity.Services.Roles;
using UzWorks.Persistence.Repositories.Chat;

namespace UzWorks.BL.Services.Chat;

public class ChatService(
    IConversationRepository _conversationRepository,
    IMessageRepository _messageRepository,
    IMappingService _mappingService,
    IUserService _userService) : IChatService
{
    public async Task<ConversationVM> StartOrGetConversationAsync(Guid currentUserId, StartConversationDto dto)
    {
        if (currentUserId == dto.OtherUserId)
            throw new UzWorksException("Cannot start a conversation with yourself.");

        var existing = await _conversationRepository.GetByParticipantsAsync(
            currentUserId, dto.OtherUserId, dto.JobId, dto.WorkerId);

        if (existing is not null)
            return await BuildConversationVM(existing, currentUserId);

        var conversation = new Conversation
        {
            Id = Guid.NewGuid(),
            ParticipantOneId = currentUserId,
            ParticipantTwoId = dto.OtherUserId,
            JobId = dto.JobId,
            WorkerId = dto.WorkerId,
            CreateDate = DateTime.Now,
            CreatedBy = currentUserId
        };

        await _conversationRepository.CreateAsync(conversation);
        await _conversationRepository.SaveChanges();

        return await BuildConversationVM(conversation, currentUserId);
    }

    public async Task<IEnumerable<ConversationVM>> GetUserConversationsAsync(Guid userId)
    {
        var conversations = await _conversationRepository.GetByUserIdAsync(userId);
        var result = new List<ConversationVM>();

        foreach (var c in conversations)
            result.Add(await BuildConversationVM(c, userId));

        return result;
    }

    public async Task<ConversationVM> GetConversationAsync(Guid conversationId, Guid userId)
    {
        var conversation = await _conversationRepository.GetWithMessagesAsync(conversationId) ??
            throw new UzWorksException($"Conversation with id {conversationId} not found.");

        if (conversation.ParticipantOneId != userId && conversation.ParticipantTwoId != userId)
            throw new UzWorksException("You do not have access to this conversation.");

        return await BuildConversationVM(conversation, userId);
    }

    public async Task<MessageVM> SendMessageAsync(Guid senderId, SendMessageDto dto)
    {
        var conversation = await _conversationRepository.GetById(dto.ConversationId) ??
            throw new UzWorksException($"Conversation with id {dto.ConversationId} not found.");

        if (conversation.ParticipantOneId != senderId && conversation.ParticipantTwoId != senderId)
            throw new UzWorksException("You do not have access to this conversation.");

        var message = new Message
        {
            Id = Guid.NewGuid(),
            ConversationId = dto.ConversationId,
            SenderId = senderId,
            Content = dto.Content,
            IsRead = false,
            CreateDate = DateTime.Now,
            CreatedBy = senderId
        };

        await _messageRepository.CreateAsync(message);
        await _messageRepository.SaveChanges();

        var senderFullName = await GetFullNameSafe(senderId);
        return ToMessageVM(message, senderFullName);
    }

    public async Task<IEnumerable<MessageVM>> GetMessagesAsync(
        Guid conversationId, Guid userId, int pageNumber, int pageSize)
    {
        var conversation = await _conversationRepository.GetById(conversationId) ??
            throw new UzWorksException($"Conversation with id {conversationId} not found.");

        if (conversation.ParticipantOneId != userId && conversation.ParticipantTwoId != userId)
            throw new UzWorksException("You do not have access to this conversation.");

        var messages = await _messageRepository.GetByConversationIdAsync(conversationId, pageNumber, pageSize);

        // Batch-fetch sender names to avoid N+1
        var senderIds = messages.Select(m => m.SenderId).Distinct();
        var nameCache = new Dictionary<Guid, string>();
        foreach (var id in senderIds)
            nameCache[id] = await GetFullNameSafe(id);

        return messages.Select(m => ToMessageVM(m, nameCache.GetValueOrDefault(m.SenderId, string.Empty)));
    }

    public async Task MarkAsReadAsync(Guid conversationId, Guid userId)
    {
        var conversation = await _conversationRepository.GetById(conversationId) ??
            throw new UzWorksException($"Conversation with id {conversationId} not found.");

        if (conversation.ParticipantOneId != userId && conversation.ParticipantTwoId != userId)
            throw new UzWorksException("You do not have access to this conversation.");

        await _messageRepository.MarkAsReadAsync(conversationId, userId);
    }

    // ─── Private helpers ────────────────────────────────────────────────────────

    private async Task<ConversationVM> BuildConversationVM(Conversation conversation, Guid currentUserId)
    {
        var otherUserId = conversation.ParticipantOneId == currentUserId
            ? conversation.ParticipantTwoId
            : conversation.ParticipantOneId;

        var otherUserFullName = await GetFullNameSafe(otherUserId);

        var lastMessage = conversation.Messages?
            .OrderByDescending(m => m.CreateDate)
            .FirstOrDefault();

        MessageVM? lastMessageVM = null;
        if (lastMessage is not null)
        {
            var senderName = await GetFullNameSafe(lastMessage.SenderId);
            lastMessageVM = ToMessageVM(lastMessage, senderName);
        }

        var unreadCount = await _messageRepository.GetUnreadCountAsync(conversation.Id, currentUserId);

        return new ConversationVM
        {
            Id = conversation.Id,
            OtherUserId = otherUserId,
            OtherUserFullName = otherUserFullName,
            JobId = conversation.JobId,
            WorkerId = conversation.WorkerId,
            CreatedAt = conversation.CreateDate,
            LastMessage = lastMessageVM,
            UnreadCount = unreadCount
        };
    }

    private static MessageVM ToMessageVM(Message m, string senderFullName) => new()
    {
        Id = m.Id,
        ConversationId = m.ConversationId,
        SenderId = m.SenderId,
        SenderFullName = senderFullName,
        Content = m.Content,
        IsRead = m.IsRead,
        SentAt = m.CreateDate
    };

    private async Task<string> GetFullNameSafe(Guid userId)
    {
        try { return await _userService.GetUserFullName(userId); }
        catch { return string.Empty; }
    }
}

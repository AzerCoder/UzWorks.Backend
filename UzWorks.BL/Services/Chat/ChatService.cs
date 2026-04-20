using UzWorks.Core.Abstract;
using UzWorks.Core.DataTransferObjects.Chat;
using UzWorks.Core.Entities.Chat;
using UzWorks.Core.Exceptions;
using UzWorks.Persistence.Repositories.Chat;

namespace UzWorks.BL.Services.Chat;

public class ChatService(
    IConversationRepository _conversationRepository,
    IMessageRepository _messageRepository,
    IMappingService _mappingService) : IChatService
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

        return ToMessageVM(message);
    }

    public async Task<IEnumerable<MessageVM>> GetMessagesAsync(Guid conversationId, Guid userId, int pageNumber, int pageSize)
    {
        var conversation = await _conversationRepository.GetById(conversationId) ??
            throw new UzWorksException($"Conversation with id {conversationId} not found.");

        if (conversation.ParticipantOneId != userId && conversation.ParticipantTwoId != userId)
            throw new UzWorksException("You do not have access to this conversation.");

        var messages = await _messageRepository.GetByConversationIdAsync(conversationId, pageNumber, pageSize);
        return messages.Select(ToMessageVM);
    }

    public async Task MarkAsReadAsync(Guid conversationId, Guid userId)
    {
        var conversation = await _conversationRepository.GetById(conversationId) ??
            throw new UzWorksException($"Conversation with id {conversationId} not found.");

        if (conversation.ParticipantOneId != userId && conversation.ParticipantTwoId != userId)
            throw new UzWorksException("You do not have access to this conversation.");

        await _messageRepository.MarkAsReadAsync(conversationId, userId);
    }

    private async Task<ConversationVM> BuildConversationVM(Conversation conversation, Guid currentUserId)
    {
        var lastMessage = conversation.Messages?
            .OrderByDescending(m => m.CreateDate)
            .FirstOrDefault();

        var unreadCount = await _messageRepository.GetUnreadCountAsync(conversation.Id, currentUserId);

        return new ConversationVM
        {
            Id = conversation.Id,
            ParticipantOneId = conversation.ParticipantOneId,
            ParticipantTwoId = conversation.ParticipantTwoId,
            JobId = conversation.JobId,
            WorkerId = conversation.WorkerId,
            CreateDate = conversation.CreateDate,
            LastMessage = lastMessage is null ? null : ToMessageVM(lastMessage),
            UnreadCount = unreadCount
        };
    }

    private static MessageVM ToMessageVM(Message m) => new()
    {
        Id = m.Id,
        ConversationId = m.ConversationId,
        SenderId = m.SenderId,
        Content = m.Content,
        IsRead = m.IsRead,
        SentAt = m.CreateDate
    };
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UzWorks.BL.Services.Chat;
using UzWorks.Core.DataTransferObjects.Chat;
using UzWorks.Core.Exceptions;

namespace UzWorks.API.Controllers;

public class ChatController(IChatService _chatService) : BaseController
{
    /// <summary>Start a new conversation or return existing one.</summary>
    [HttpPost]
    public async Task<ActionResult<ConversationVM>> StartConversation([FromBody] StartConversationDto dto)
    {
        var userId = GetCurrentUserId();
        return Ok(await _chatService.StartOrGetConversationAsync(userId, dto));
    }

    /// <summary>Get all conversations for the current user.</summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ConversationVM>>> GetConversations()
    {
        var userId = GetCurrentUserId();
        return Ok(await _chatService.GetUserConversationsAsync(userId));
    }

    /// <summary>Get a single conversation with its messages.</summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ConversationVM>> GetConversation([FromRoute] Guid id)
    {
        var userId = GetCurrentUserId();
        return Ok(await _chatService.GetConversationAsync(id, userId));
    }

    /// <summary>Send a message (REST fallback — prefer SignalR hub).</summary>
    [HttpPost]
    public async Task<ActionResult<MessageVM>> SendMessage([FromBody] SendMessageDto dto)
    {
        var userId = GetCurrentUserId();
        return Ok(await _chatService.SendMessageAsync(userId, dto));
    }

    /// <summary>Get paginated messages for a conversation.</summary>
    [HttpGet("{conversationId}")]
    public async Task<ActionResult<IEnumerable<MessageVM>>> GetMessages(
        [FromRoute] Guid conversationId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 50)
    {
        var userId = GetCurrentUserId();
        return Ok(await _chatService.GetMessagesAsync(conversationId, userId, pageNumber, pageSize));
    }

    /// <summary>Mark all unread messages in a conversation as read.</summary>
    [HttpPut("{conversationId}")]
    public async Task<ActionResult> MarkAsRead([FromRoute] Guid conversationId)
    {
        var userId = GetCurrentUserId();
        await _chatService.MarkAsReadAsync(conversationId, userId);
        return Ok();
    }

    private Guid GetCurrentUserId()
    {
        var claim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ??
            throw new UzWorksException("User is not authenticated.");
        return Guid.Parse(claim);
    }
}

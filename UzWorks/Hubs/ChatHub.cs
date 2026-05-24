using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using UzWorks.BL.Services.Chat;
using UzWorks.Core.DataTransferObjects.Chat;
using UzWorks.Identity.Constants;

namespace UzWorks.API.Hubs;

[Authorize]
public class ChatHub(IChatService _chatService) : Hub
{
    public override async Task OnConnectedAsync()
    {
        var userId = Context.User?.FindFirst(ClaimNames.UserId)?.Value;
        if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out _))
        {
            Context.Abort();
            return;
        }
        await base.OnConnectedAsync();
    }

    /// <summary>
    /// Join a conversation room.
    /// Marks all unread messages as read and adds the connection to the group.
    /// Optional — messages are delivered via Clients.Users() regardless of group membership.
    /// </summary>
    public async Task JoinConversation(Guid conversationId)
    {
        var userId = GetUserId();
        await _chatService.MarkAsReadAsync(conversationId, userId);
        await Groups.AddToGroupAsync(Context.ConnectionId, conversationId.ToString());
    }

    public async Task LeaveConversation(Guid conversationId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, conversationId.ToString());
    }

    /// <summary>
    /// Send a message. Delivered to BOTH participants by UserId —
    /// no need for the recipient to have called JoinConversation first.
    /// </summary>
    public async Task SendMessage(Guid conversationId, string content)
    {
        var senderId = GetUserId();
        var dto = new SendMessageDto { ConversationId = conversationId, Content = content };

        // Save to DB
        var message = await _chatService.SendMessageAsync(senderId, dto);

        // Get both participant IDs
        var (p1, p2) = await _chatService.GetParticipantIdsAsync(conversationId);

        // Deliver to ALL active connections of both users — regardless of JoinConversation
        await Clients
            .Users(p1.ToString(), p2.ToString())
            .SendAsync("ReceiveMessage", message);
    }

    private Guid GetUserId()
    {
        var userIdStr = Context.User?.FindFirst(ClaimNames.UserId)?.Value
            ?? throw new HubException("User is not authenticated.");

        if (!Guid.TryParse(userIdStr, out var userId))
            throw new HubException($"Invalid UserId format: '{userIdStr}'.");

        return userId;
    }
}

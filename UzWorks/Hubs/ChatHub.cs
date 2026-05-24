using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using UzWorks.BL.Services.Chat;
using UzWorks.Core.DataTransferObjects.Chat;
using UzWorks.Identity.Constants;

namespace UzWorks.API.Hubs;

/// <summary>
/// Real-time chat hub.
///
/// Client events received (invoke):
///   JoinConversation(conversationId)   — mark messages read, notify sender
///   SendMessage(conversationId, text)  — save + deliver to both participants
///   LeaveConversation(conversationId)  — optional group leave
///
/// Client events emitted (on):
///   ReceiveMessage    — new message object (MessageVM)
///   MessagesRead      — { conversationId, readByUserId } — sender's messages were read
///   Error             — string error message
/// </summary>
[Authorize]
public class ChatHub(IChatService _chatService) : Hub
{
    // ─── Connection lifecycle ───────────────────────────────────────────────────

    public override async Task OnConnectedAsync()
    {
        var userId = GetUserIdOrNull();
        if (userId is null)
        {
            Context.Abort();
            return;
        }
        await base.OnConnectedAsync();
    }

    // ─── Hub methods ────────────────────────────────────────────────────────────

    /// <summary>
    /// Call when the user opens a conversation screen.
    /// Marks all unread messages as read and notifies message senders (read-receipt).
    /// </summary>
    public async Task JoinConversation(Guid conversationId)
    {
        var userId = GetUserId();

        // Mark messages as read; get sender IDs whose messages were marked
        IReadOnlyList<Guid> senderIds;
        try
        {
            senderIds = await _chatService.MarkAsReadAsync(conversationId, userId);
        }
        catch (Exception ex)
        {
            await Clients.Caller.SendAsync("Error", ex.Message);
            return;
        }

        await Groups.AddToGroupAsync(Context.ConnectionId, conversationId.ToString());

        // Notify each sender: their messages were read
        var readEvent = new { conversationId, readByUserId = userId };
        foreach (var senderId in senderIds.Where(id => id != userId))
            await Clients.User(senderId.ToString()).SendAsync("MessagesRead", readEvent);
    }

    /// <summary>
    /// Send a message. Delivered in real-time to BOTH participants by user ID,
    /// so the recipient does NOT need to have called JoinConversation first.
    /// </summary>
    public async Task SendMessage(Guid conversationId, string content)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            await Clients.Caller.SendAsync("Error", "Message content cannot be empty.");
            return;
        }

        var senderId = GetUserId();

        MessageVM message;
        (Guid p1, Guid p2) participants;

        try
        {
            var dto = new SendMessageDto { ConversationId = conversationId, Content = content };
            message = await _chatService.SendMessageAsync(senderId, dto);
            participants = await _chatService.GetParticipantIdsAsync(conversationId);
        }
        catch (Exception ex)
        {
            await Clients.Caller.SendAsync("Error", ex.Message);
            return;
        }

        // Deliver to ALL active connections of both participants
        // (independent of JoinConversation / group membership)
        await Clients
            .Users(participants.p1.ToString(), participants.p2.ToString())
            .SendAsync("ReceiveMessage", message);
    }

    /// <summary>
    /// Call when the user leaves the conversation screen.
    /// </summary>
    public async Task LeaveConversation(Guid conversationId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, conversationId.ToString());
    }

    // ─── Helpers ────────────────────────────────────────────────────────────────

    private Guid GetUserId()
    {
        var userIdStr = Context.User?.FindFirst(ClaimNames.UserId)?.Value
            ?? throw new HubException("User is not authenticated.");

        if (!Guid.TryParse(userIdStr, out var userId))
            throw new HubException($"Invalid UserId format: '{userIdStr}'.");

        return userId;
    }

    private Guid? GetUserIdOrNull()
    {
        var val = Context.User?.FindFirst(ClaimNames.UserId)?.Value;
        return Guid.TryParse(val, out var id) ? id : null;
    }
}

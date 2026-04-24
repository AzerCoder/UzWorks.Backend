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

    public async Task SendMessage(Guid conversationId, string content)
    {
        var userId = GetUserId();
        var dto = new SendMessageDto { ConversationId = conversationId, Content = content };
        var message = await _chatService.SendMessageAsync(userId, dto);
        await Clients.Group(conversationId.ToString()).SendAsync("ReceiveMessage", message);
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

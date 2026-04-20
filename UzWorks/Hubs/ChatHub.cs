using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using UzWorks.BL.Services.Chat;
using UzWorks.Core.DataTransferObjects.Chat;

namespace UzWorks.API.Hubs;

[Authorize]
public class ChatHub(IChatService _chatService) : Hub
{
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
        var userIdStr = Context.UserIdentifier ??
            throw new HubException("User is not authenticated.");
        return Guid.Parse(userIdStr);
    }
}

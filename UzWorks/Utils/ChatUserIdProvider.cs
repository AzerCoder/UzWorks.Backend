using Microsoft.AspNetCore.SignalR;
using UzWorks.Identity.Constants;

namespace UzWorks.API.Utils;

/// <summary>
/// Tells SignalR to use the custom "UserId" claim as the user identifier
/// instead of the default ClaimTypes.NameIdentifier (which holds the phone number).
/// </summary>
public class ChatUserIdProvider : IUserIdProvider
{
    public string? GetUserId(HubConnectionContext connection)
        => connection.User.FindFirst(ClaimNames.UserId)?.Value;
}

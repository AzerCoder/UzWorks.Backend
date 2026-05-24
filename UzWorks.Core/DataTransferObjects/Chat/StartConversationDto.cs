using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace UzWorks.Core.DataTransferObjects.Chat;

public class StartConversationDto
{
    /// <summary>
    /// The other participant's user ID.
    /// Accepts both "otherUserId" and "targetUserId" from JSON body.
    /// </summary>
    [Required]
    [JsonPropertyName("targetUserId")]
    public Guid OtherUserId { get; set; }

    public Guid? JobId { get; set; }
    public Guid? WorkerId { get; set; }
}

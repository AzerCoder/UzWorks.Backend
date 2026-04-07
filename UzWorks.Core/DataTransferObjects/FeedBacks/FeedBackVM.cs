namespace UzWorks.Core.DataTransferObjects.FeedBacks;

public class FeedBackVM
{
    public Guid Id { get; set; }
    public string Message { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public DateTime DueDate { get; set; }
}

namespace UzWorks.Core.Entities.Advertisements;

public class Advertisement : BaseEntity
{
    public string Title { get; set; }
    public string Description { get; set; }
    public string? ImageUrl { get; set; }
    public string? IconUrl { get; set; }
    public string RedirectUrl { get; set; }
}

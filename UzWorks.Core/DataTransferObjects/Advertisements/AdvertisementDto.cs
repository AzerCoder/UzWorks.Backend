namespace UzWorks.Core.DataTransferObjects.Advertisements;

public class AdvertisementDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public string? IconUrl { get; set; }
    public string RedirectUrl { get; set; } = string.Empty;
}

namespace UzWorks.Core.DataTransferObjects.Contacts;

public class ContactVM
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public DateTime CreateDate { get; set; }
    public bool IsComplated { get; set; }
}

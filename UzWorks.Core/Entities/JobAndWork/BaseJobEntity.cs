using UzWorks.Core.Enums.GenderTypes;

namespace UzWorks.Core.Entities.JobAndWork;

public class BaseJobEntity : BaseEntity
{
    public string Title { get; set; }
    public uint Salary {  get; set; }
    public GenderEnum Gender { get; set; } = GenderEnum.Unknown;
    public string WorkingTime { get; set; }
    public string WorkingSchedule { get; set; }
    public bool? Status { get; set; } = null;
    public bool IsTop { get; set; } = false;
    public DateTime Deadline { get; set; }
    public string? TelegramLink { get; set; }
    public string? InstagramLink { get; set; }
    public string TgUserName { get; set; }
    public string PhoneNumber { get; set; }
}

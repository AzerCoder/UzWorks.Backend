namespace UzWorks.Core.Entities.JobAndWork;

public class JobCategory : BaseEntity
{
    public JobCategory(string title, string description)
    {
        Title = title;
        Description = description;
    }

    public string Title { get; set; }
    public string? Description { get; set; }

    public List<Job>? Jobs{ get; set; }
    public List<Worker>? Works { get; set; }

}
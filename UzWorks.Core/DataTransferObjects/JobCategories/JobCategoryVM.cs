namespace UzWorks.Core.DataTransferObjects.JobCategories;

public class JobCategoryVM
{
    public Guid? Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int CountOfWorkers { get; set; } = 0;
    public int CountOfJobs { get; set; } = 0;
}

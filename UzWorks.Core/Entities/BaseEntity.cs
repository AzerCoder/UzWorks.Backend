namespace UzWorks.Core.Entities;

public class BaseEntity : IHasIsDeletedProperty
{
    public Guid Id { get; set; }
    public DateTime? CreateDate { get; set; } = DateTime.Now;
    public DateTime? UpdateDate { get; set; }
    public Guid? CreatedBy { get; set; }
    public Guid? UpdatedBy { get; set; }
    public bool IsDeleted { get; set; } = false;
}

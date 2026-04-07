namespace UzWorks.Core.Entities.Location;

public class Region : BaseEntity
{
    public Region(string name) =>
        Name = name;

    public string Name { get; set; }
    
    public List<District>? Districts { get; set; }
}

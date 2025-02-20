using Exptour.Domain.Entities.Common;

namespace Exptour.Domain.Entities;

public class Category : BaseEntity
{
    public string NameEN { get; set; } = null!;
    public string NameAR { get; set; } = null!;
    public string? Description { get; set; }

    //RELATIONSHIPs
    public ICollection<Tour> Tours { get; set; }
}

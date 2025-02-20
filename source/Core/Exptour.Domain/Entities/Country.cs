using Exptour.Domain.Entities.Common;

namespace Exptour.Domain.Entities;

public class Country : BaseEntity
{
    public string Name { get; set; } = null!;

    //RELATIONSHIPs
    public ICollection<City> Cities { get; set; }
}

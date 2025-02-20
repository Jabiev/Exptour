using Exptour.Domain.Entities.Common;

namespace Exptour.Domain.Entities;

public class CarBrand : BaseEntity
{
    public string Name { get; set; } = null!;

    //RELATIONSHIPs
    public ICollection<CarModel> CarModels { get; set; }
}

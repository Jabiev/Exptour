using Exptour.Domain.Entities.Common;
using Microsoft.EntityFrameworkCore;

namespace Exptour.Domain.Entities;

public class CarModel : BaseEntity
{
    public string Name { get; set; } = null!;
    public Guid CarBrandId { get; set; }

    //RELATIONSHIPs
    public CarBrand CarBrand { get; set; }
    public ICollection<Car> Cars { get; set; }
}

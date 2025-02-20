using Exptour.Domain.Entities.Common;
using Microsoft.EntityFrameworkCore;

namespace Exptour.Domain.Entities;

public class City : BaseEntity
{
    public string Name { get; set; } = null!;
    public Guid CountryId { get; set; }

    //RELATIONSHIPs
    public Country Country { get; set; }
    public ICollection<Tour> Tours { get; set; }
    public ICollection<Hotel> Hotels { get; set; }
}

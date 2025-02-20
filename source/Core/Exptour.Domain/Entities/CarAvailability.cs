using Exptour.Domain.Entities.Common;

namespace Exptour.Domain.Entities;

public class CarAvailability : BaseEntity
{
    public Guid CarId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public CarStatus Status { get; set; } = CarStatus.Available;

    //RELATIONSHIPs
    public Car Car { get; set; }
}

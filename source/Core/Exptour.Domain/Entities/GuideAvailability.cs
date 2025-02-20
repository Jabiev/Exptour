using Exptour.Domain.Entities.Common;

namespace Exptour.Domain.Entities;

public class GuideAvailability : BaseEntity
{
    public Guid GuideId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsAvailable { get; set; }

    //RELATIONSHIPs
    public Guide Guide { get; set; }
}

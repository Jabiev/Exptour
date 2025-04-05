using Exptour.Domain.Entities.Common;

namespace Exptour.Domain.Entities;

public class GuideSchedule : BaseEntity
{
    public Guid GuideId { get; set; }
    //something that belongs to tours the guide attended will be in this table
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    //RELATIONSHIPs
    public Guide Guide { get; set; }
}

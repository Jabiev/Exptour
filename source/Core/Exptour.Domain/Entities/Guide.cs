using Exptour.Domain.Entities.Common;

namespace Exptour.Domain.Entities;

public class Guide : BaseEntity
{
    public string FullName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public bool IsAvailable { get; set; } = true;
    public GuideStatus Status { get; set; } = GuideStatus.Active;
    public Gender Gender { get; set; }

    //RELATIONSHIPs
    public ICollection<Language> Languages { get; set; }
    public ICollection<GuideSchedule> GuideSchedules { get; set; }
    public ICollection<Offer> Offers { get; set; }
    public ICollection<Booking> Bookings { get; set; }
}

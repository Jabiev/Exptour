using Exptour.Domain.Entities.Common;

namespace Exptour.Domain.Entities;

public class Guide : BaseEntity
{
    public string FullName { get; set; } = null!;

    //RELATIONSHIPs
    public ICollection<Language> Languages { get; set; }
    public ICollection<GuideAvailability> Availabilities { get; set; }
    public ICollection<Offer> Offers { get; set; }
    public ICollection<Booking> Bookings { get; set; }
}

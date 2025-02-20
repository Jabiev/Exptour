using Exptour.Domain.Entities.Common;

namespace Exptour.Domain.Entities;

public class Offer : BaseEntity
{
    public Guid ApplicationUserId { get; set; }
    public Guid TourId { get; set; }
    public Guid HotelId { get; set; }
    public bool IsChosenGuide { get; set; }
    public Guid? GuideId { get; set; }
    public bool IsChosenCar { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int NumberOfGuests { get; set; }
    public decimal EstimatedPrice { get; set; } // to TotalPrice in Booking
    public OfferStatus Status { get; set; } = OfferStatus.Pending;

    //RELATIONSHIPs
    public Tour Tour { get; set; }
    public Hotel Hotel { get; set; }
    public Guide? Guide { get; set; }
    public Booking Booking { get; set; }
    public ApplicationUser ApplicationUser { get; set; }
    public ICollection<Car>? Cars { get; set; }
}

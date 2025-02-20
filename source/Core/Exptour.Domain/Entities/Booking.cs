using Exptour.Domain.Entities.Common;

namespace Exptour.Domain.Entities;

public class Booking : BaseEntity
{
    public Guid ApplicationUserId { get; set; }
    public Guid TourId { get; set; }
    public Guid HotelId { get; set; }
    public bool IsChosenGuide { get; set; }
    public Guid? GuideId { get; set; }
    public bool IsChosenCar { get; set; }
    public DateTime StartDate { get; set; } //Valid interval
    public DateTime EndDate { get; set; } // of Booking
    public int NumberOfGuests { get; set; }
    public decimal TotalPrice { get; set; }
    public BookingStatus Status { get; set; }
    public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;
    public Guid? OfferId { get; set; }

    //RELATIONSHIPs
    public Tour Tour { get; set; }
    public Hotel Hotel { get; set; }
    public Offer? Offer { get; set; }
    public Guide? Guide { get; set; }
    public Payment Payment { get; set; }
    public ICollection<Car>? Cars { get; set; }
    public ApplicationUser ApplicationUser { get; set; }
}

using Exptour.Domain.Entities.Common;
using Microsoft.EntityFrameworkCore;

namespace Exptour.Domain.Entities;

public class Hotel : BaseEntity
{
    public string Name { get; set; }

    [Precision(9, 6)]
    public decimal Latitude { get; set; }

    [Precision(9, 6)]
    public decimal Longitude { get; set; }
    public string? Address { get; set; } // Extended Adress(City.Name+)
    public string BannerImage { get; set; }
    public AvailabilityStatus Availability { get; set; } = AvailabilityStatus.Available;

    [Precision(4, 1)]
    public decimal DiscountPercentage { get; set; } //sometimes there might be discount and customers can search according to the discount : > 0
    public Guid ApplicationUserId { get; set; }
    public Guid CityId { get; set; }

    //RELATIONSHIPs
    public City City { get; set; }
    public ApplicationUser ApplicationUser { get; set; }
    public ICollection<Room> Rooms { get; set; }
    public ICollection<Offer> Offers { get; set; }
    public ICollection<Booking> Bookings { get; set; }
    public ICollection<HotelImage> HotelImages { get; set; }
}

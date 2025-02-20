using Exptour.Domain.Entities.Common;
using Microsoft.EntityFrameworkCore;

namespace Exptour.Domain.Entities;

public class Tour : BaseEntity
{
    public string BannerImage { get; set; } = null!; //Firsly Image which being firstly visible
    public string NameEN { get; set; } = null!;
    public string NameAR { get; set; }
    public string? Description { get; set; }
    public string ProviderLicense { get; set; } = null!; //UK-TRAVEL-98765
    public TimeSpan Duration { get; set; } //8 days etc.
    public decimal Price { get; set; }
    public TourStatus Status { get; set; }
    public Currency Currency { get; set; }
    public int MaxPersonCount { get; set; }
    public TimeOfDay TimeOfDay { get; set; }
    public bool IsActive { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    [Precision(4, 1)]
    public decimal DiscountPercentage { get; set; } //sometimes there might be discount and customers can search according to the discount : > 0
    public bool IsHotelPickUp { get; set; } //continuable for thriving project in the future
    public Point MeetingPoint { get; set; }
    public Point EndPoint { get; set; }
    public Guid CityId { get; set; }
    public Guid ApplicationUserId { get; set; }

    //RELATIONSHIPs
    public City City { get; set; }
    public ApplicationUser ApplicationUser { get; set; }
    public ICollection<Offer> Offers { get; set; }
    public ICollection<Booking> Bookings { get; set; }
    public ICollection<Category> Categories { get; set; }
    public ICollection<TourImage> TourImages { get; set; }
}

public class Point
{
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
}

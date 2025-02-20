using Exptour.Domain.Entities.Common;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Exptour.Domain.Entities;

/// <summary>
/// Real Car(Vehicle)
/// </summary>
public class Car : BaseEntity
{
    public Guid CarModelId { get; set; }

    [RegularExpression(@"^[A-Z0-9\- ]{5,10}$")]
    public string PlateNumber { get; set; }
    public DateOnly Year { get; set; }
    public int CountOfSeats { get; set; }

    [Precision(7, 2)]
    public decimal PricePerDay { get; set; }

    //RELATIONSHIPs
    public CarModel CarModel { get; set; }
    public ICollection<Booking> Booking { get; set; }
    public ICollection<Offer> Offers { get; set; }
    public ICollection<CarAvailability> Availabilities { get; set; }
    public ICollection<CarImage> CarImages { get; set; }
}

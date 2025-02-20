using Exptour.Domain.Entities.Common;

namespace Exptour.Domain.Entities;

public class Room : BaseEntity
{
    public string RoomNumber { get; set; }
    public int Capacity { get; set; }
    public decimal PricePerNight { get; set; }
    public bool IsAvailable { get; set; }
    public RoomType RoomType { get; set; }
    public Guid HotelId { get; set; }

    //RELATIONSHIPs
    public Hotel Hotel { get; set; }
    public ICollection<RoomImage> RoomImages { get; set; }
}

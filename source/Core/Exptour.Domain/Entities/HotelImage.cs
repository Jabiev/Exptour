using Exptour.Domain.Entities.Common;

namespace Exptour.Domain.Entities;

public class HotelImage : BaseEntity
{
    public string Url { get; set; } = null!; //Blob Storage URL
    public ImageType ImageType { get; set; }
    public Guid HotelId { get; set; }

    //RELATIONSHIPs
    public Hotel Hotel { get; set; }
}

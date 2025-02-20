using Exptour.Domain.Entities.Common;

namespace Exptour.Domain.Entities;

public class RoomImage : BaseEntity
{
    public string Url { get; set; } = null!; //Blob Storage URL
    public ImageType ImageType { get; set; }
    public Guid RoomId { get; set; }

    //RELATIONSHIPs
    public Room Room { get; set; }
}

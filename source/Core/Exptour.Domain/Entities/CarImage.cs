using Exptour.Domain.Entities.Common;

namespace Exptour.Domain.Entities;

public class CarImage : BaseEntity
{
    public string Url { get; set; } = null!; //Blob Storage URL
    public ImageType ImageType { get; set; }
    public Guid CarId { get; set; }

    //RELATIONSHIPs
    public Car Car { get; set; }
}

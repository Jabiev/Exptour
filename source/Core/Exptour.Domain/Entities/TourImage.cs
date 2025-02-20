using Exptour.Domain.Entities.Common;

namespace Exptour.Domain.Entities;

public class TourImage : BaseEntity
{
    public string Url { get; set; } = null!; //Blob Storage URL
    public ImageType ImageType { get; set; }
    public Guid TourId { get; set; }

    //RELATIONSHIPs
    public Tour Tour { get; set; }
}

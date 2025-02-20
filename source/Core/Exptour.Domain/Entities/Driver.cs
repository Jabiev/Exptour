using Exptour.Domain.Entities.Common;
using System.ComponentModel.DataAnnotations;

namespace Exptour.Domain.Entities;

public class Driver : BaseEntity
{
    public string FullName { get; set; } = null!;

    [RegularExpression(@"^\+?[1-9]\d{1,14}$")]
    public string PhoneNumber { get; set; }
    public bool IsAvailable { get; set; } = true;
    public Guid CarId { get; set; }

    //RELATIONSHIPs
    public Car Car { get; set; }
}

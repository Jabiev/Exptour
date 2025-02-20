using Exptour.Domain.Entities.Common;
using System.ComponentModel.DataAnnotations;

namespace Exptour.Domain.Entities;

public class Language : BaseEntity
{
    public string NameEN { get; set; } = null!;
    public string NameAR { get; set; } = null!;

    [RegularExpression(@"^[a-z]{2,3}$")]
    public string Code { get; set; } = null!;

    //RELATIONSHIPs
    public ICollection<Guide> Guides { get; set; }
}

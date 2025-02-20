using Exptour.Domain.Entities.Common;

namespace Exptour.Domain.Entities;

public class Permission : BaseEntity
{
    public string NameEN { get; set; } = null!;
    public string NameAR { get; set; } = null!;
    public string? Description { get; set; }

    //RELATIONSHIPs
    public ICollection<RolePermission> RolePermissions { get; set; }
}

using Exptour.Domain.Entities.Common;
using Microsoft.AspNetCore.Identity;

namespace Exptour.Domain.Entities;

public class RolePermission : BaseEntity
{
    public Guid RoleId { get; set; }
    public Guid PermissionId { get; set; }

    //RELATIONSHIPs
    public IdentityRole<Guid> Role { get; set; }
    public Permission Permission { get; set; }
}

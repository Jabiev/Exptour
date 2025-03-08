using Exptour.Domain.Entities.Common;
using Microsoft.AspNetCore.Identity;

namespace Exptour.Domain.Entities;

public class EndpointRole : BaseEntity
{
    public Guid EndpointId { get; set; }
    public Endpoint Endpoint { get; set; }

    public Guid RoleId { get; set; }
    public IdentityRole<Guid> Role { get; set; }
}

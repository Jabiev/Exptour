using Exptour.Domain.Entities.Common;
using Microsoft.AspNetCore.Identity;

namespace Exptour.Domain.Entities;

public class Endpoint : BaseEntity
{
    public Endpoint()
    {
        EndpointRoles = new HashSet<EndpointRole>();
    }

    public string ActionType { get; set; }
    public string HttpType { get; set; }
    public string Definition { get; set; }
    public string Code { get; set; }
    public Guid MenuId { get; set; }

    //RELATIONSHIPs
    public Menu Menu { get; set; }
    public ICollection<EndpointRole> EndpointRoles { get; set; }
}

using Exptour.Application.Abstract.Repositories.EndpointRoles;
using Exptour.Domain.Entities;
using Exptour.Persistence.Contexts;

namespace Exptour.Persistence.Concrete.Repositories.EndpointRoles;

public class EndpointRoleWriteRepository : WriteRepository<EndpointRole>, IEndpointRoleWriteRepository
{
    public EndpointRoleWriteRepository(TourismManagementDbContext tourismManagementDbContext) : base(tourismManagementDbContext)
    {
    }
}

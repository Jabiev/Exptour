using Exptour.Application.Abstract.Repositories.EndpointRoles;
using Exptour.Domain.Entities;
using Exptour.Persistence.Contexts;

namespace Exptour.Persistence.Concrete.Repositories.EndpointRoles;

public class EndpointRoleReadRepository : ReadRepository<EndpointRole>, IEndpointRoleReadRepository
{
    public EndpointRoleReadRepository(TourismManagementDbContext tourismManagementDbContext) : base(tourismManagementDbContext)
    {
    }
}

using Exptour.Application.Abstract.Repositories.Endpoints;
using Exptour.Domain.Entities;
using Exptour.Persistence.Contexts;

namespace Exptour.Persistence.Concrete.Repositories.Endpoints;

public class EndpointWriteRepository : WriteRepository<Endpoint>, IEndpointWriteRepository
{
    public EndpointWriteRepository(TourismManagementDbContext tourismManagementDbContext) : base(tourismManagementDbContext)
    {
    }
}

using Exptour.Application.Abstract.Repositories.Endpoints;
using Exptour.Domain.Entities;
using Exptour.Persistence.Contexts;

namespace Exptour.Persistence.Concrete.Repositories.Endpoints;

public class EndpointReadRepository : ReadRepository<Endpoint>, IEndpointReadRepository
{
    public EndpointReadRepository(TourismManagementDbContext tourismManagementDbContext) : base(tourismManagementDbContext)
    {
    }
}

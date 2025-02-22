using Exptour.Application.Abstract.Repositories.Tours;
using Exptour.Domain.Entities;
using Exptour.Persistence.Contexts;

namespace Exptour.Persistence.Concrete.Repositories.Tours;

public class TourWriteRepository : WriteRepository<Tour>, ITourWriteRepository
{
    public TourWriteRepository(TourismManagementDbContext tourismManagementDbContext) : base(tourismManagementDbContext)
    {
    }
}

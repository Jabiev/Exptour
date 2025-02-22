using Exptour.Application.Abstract.Repositories.Drivers;
using Exptour.Domain.Entities;
using Exptour.Persistence.Contexts;

namespace Exptour.Persistence.Concrete.Repositories.Drivers;

public class DriverReadRepository : ReadRepository<Driver>, IDriverReadRepository
{
    public DriverReadRepository(TourismManagementDbContext tourismManagementDbContext) : base(tourismManagementDbContext)
    {
    }
}

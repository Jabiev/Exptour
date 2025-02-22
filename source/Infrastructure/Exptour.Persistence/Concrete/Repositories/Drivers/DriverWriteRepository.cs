using Exptour.Application.Abstract.Repositories.Drivers;
using Exptour.Domain.Entities;
using Exptour.Persistence.Contexts;

namespace Exptour.Persistence.Concrete.Repositories.Drivers;

public class DriverWriteRepository : WriteRepository<Driver>, IDriverWriteRepository
{
    public DriverWriteRepository(TourismManagementDbContext tourismManagementDbContext) : base(tourismManagementDbContext)
    {
    }
}

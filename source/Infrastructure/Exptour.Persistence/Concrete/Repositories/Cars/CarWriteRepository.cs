using Exptour.Application.Abstract.Repositories.Cars;
using Exptour.Domain.Entities;
using Exptour.Persistence.Contexts;

namespace Exptour.Persistence.Concrete.Repositories.Cars;

public class CarWriteRepository : WriteRepository<Car>, ICarWriteRepository
{
    public CarWriteRepository(TourismManagementDbContext tourismManagementDbContext) : base(tourismManagementDbContext)
    {
    }
}

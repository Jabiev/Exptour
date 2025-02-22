using Exptour.Application.Abstract.Repositories.CarAvailabilities;
using Exptour.Domain.Entities;
using Exptour.Persistence.Contexts;

namespace Exptour.Persistence.Concrete.Repositories.CarAvailabilities;

public class CarAvailabilityWriteRepository : WriteRepository<CarAvailability>, ICarAvailabilityWriteRepository
{
    public CarAvailabilityWriteRepository(TourismManagementDbContext tourismManagementDbContext) : base(tourismManagementDbContext)
    {
    }
}

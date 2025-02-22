using Exptour.Application.Abstract.Repositories.CarAvailabilities;
using Exptour.Domain.Entities;
using Exptour.Persistence.Contexts;

namespace Exptour.Persistence.Concrete.Repositories.CarAvailabilities;

public class CarAvailabilityReadRepository : ReadRepository<CarAvailability>, ICarAvailabilityReadRepository
{
    public CarAvailabilityReadRepository(TourismManagementDbContext tourismManagementDbContext) : base(tourismManagementDbContext)
    {
    }
}

using Exptour.Application.Abstract.Repositories.CarImages;
using Exptour.Domain.Entities;
using Exptour.Persistence.Contexts;

namespace Exptour.Persistence.Concrete.Repositories.CarImages;

public class CarImageReadRepository : ReadRepository<CarImage>, ICarImageReadRepository
{
    public CarImageReadRepository(TourismManagementDbContext tourismManagementDbContext) : base(tourismManagementDbContext)
    {
    }
}

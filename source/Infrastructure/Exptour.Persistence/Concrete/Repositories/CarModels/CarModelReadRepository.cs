using Exptour.Application.Abstract.Repositories.CarModels;
using Exptour.Domain.Entities;
using Exptour.Persistence.Contexts;

namespace Exptour.Persistence.Concrete.Repositories.CarModels;

public class CarModelReadRepository : ReadRepository<CarModel>, ICarModelReadRepository
{
    public CarModelReadRepository(TourismManagementDbContext tourismManagementDbContext) : base(tourismManagementDbContext)
    {
    }
}

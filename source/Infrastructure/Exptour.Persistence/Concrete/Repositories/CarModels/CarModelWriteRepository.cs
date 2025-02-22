using Exptour.Application.Abstract.Repositories.CarModels;
using Exptour.Domain.Entities;
using Exptour.Persistence.Contexts;

namespace Exptour.Persistence.Concrete.Repositories.CarModels;

public class CarModelWriteRepository : WriteRepository<CarModel>, ICarModelWriteRepository
{
    public CarModelWriteRepository(TourismManagementDbContext tourismManagementDbContext) : base(tourismManagementDbContext)
    {
    }
}

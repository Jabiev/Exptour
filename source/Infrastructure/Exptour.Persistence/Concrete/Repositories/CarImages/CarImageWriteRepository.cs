using Exptour.Application.Abstract.Repositories.CarImages;
using Exptour.Domain.Entities;
using Exptour.Persistence.Contexts;

namespace Exptour.Persistence.Concrete.Repositories.CarImages;

public class CarImageWriteRepository : WriteRepository<CarImage>, ICarImageWriteRepository
{
    public CarImageWriteRepository(TourismManagementDbContext tourismManagementDbContext) : base(tourismManagementDbContext)
    {
    }
}

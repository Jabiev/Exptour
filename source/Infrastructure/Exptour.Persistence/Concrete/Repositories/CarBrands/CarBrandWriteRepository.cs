using Exptour.Application.Abstract.Repositories.CarBrands;
using Exptour.Domain.Entities;
using Exptour.Persistence.Contexts;

namespace Exptour.Persistence.Concrete.Repositories.CarBrands;

public class CarBrandWriteRepository : WriteRepository<CarBrand>, ICarBrandWriteRepository
{
    public CarBrandWriteRepository(TourismManagementDbContext tourismManagementDbContext) : base(tourismManagementDbContext)
    {
    }
}

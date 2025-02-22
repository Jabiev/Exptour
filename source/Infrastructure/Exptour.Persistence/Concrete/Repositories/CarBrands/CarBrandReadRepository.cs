using Exptour.Application.Abstract.Repositories.CarBrands;
using Exptour.Domain.Entities;
using Exptour.Persistence.Contexts;

namespace Exptour.Persistence.Concrete.Repositories.CarBrands;

public class CarBrandReadRepository : ReadRepository<CarBrand>, ICarBrandReadRepository
{
    public CarBrandReadRepository(TourismManagementDbContext tourismManagementDbContext) : base(tourismManagementDbContext)
    {
    }
}

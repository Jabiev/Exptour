using Exptour.Application.Abstract.Repositories.Cities;
using Exptour.Domain.Entities;
using Exptour.Persistence.Contexts;

namespace Exptour.Persistence.Concrete.Repositories.Cities;

public class CityWriteRepository : WriteRepository<City>, ICityWriteRepository
{
    public CityWriteRepository(TourismManagementDbContext tourismManagementDbContext) : base(tourismManagementDbContext)
    {
    }
}

using Exptour.Application.Abstract.Repositories.Cities;
using Exptour.Domain.Entities;
using Exptour.Persistence.Contexts;

namespace Exptour.Persistence.Concrete.Repositories.Cities;

public class CityReadRepository : ReadRepository<City>, ICityReadRepository
{
    public CityReadRepository(TourismManagementDbContext tourismManagementDbContext) : base(tourismManagementDbContext)
    {
    }
}

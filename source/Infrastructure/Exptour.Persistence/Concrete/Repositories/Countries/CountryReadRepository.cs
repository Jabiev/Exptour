using Exptour.Application.Abstract.Repositories.Countries;
using Exptour.Domain.Entities;
using Exptour.Persistence.Contexts;

namespace Exptour.Persistence.Concrete.Repositories.Countries;

public class CountryReadRepository : ReadRepository<Country>, ICountryReadRepository
{
    public CountryReadRepository(TourismManagementDbContext tourismManagementDbContext) : base(tourismManagementDbContext)
    {
    }
}

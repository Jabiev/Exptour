using Exptour.Application.Abstract.Repositories.Countries;
using Exptour.Domain.Entities;
using Exptour.Persistence.Contexts;

namespace Exptour.Persistence.Concrete.Repositories.Countries;

public class CountryWriteRepository : WriteRepository<Country>, ICountryWriteRepository
{
    public CountryWriteRepository(TourismManagementDbContext tourismManagementDbContext) : base(tourismManagementDbContext)
    {
    }
}

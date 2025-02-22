using Exptour.Application.Abstract.Repositories.Hotels;
using Exptour.Domain.Entities;
using Exptour.Persistence.Contexts;

namespace Exptour.Persistence.Concrete.Repositories.Hotels;

public class HotelWriteRepository : WriteRepository<Hotel>, IHotelWriteRepository
{
    public HotelWriteRepository(TourismManagementDbContext tourismManagementDbContext) : base(tourismManagementDbContext)
    {
    }
}

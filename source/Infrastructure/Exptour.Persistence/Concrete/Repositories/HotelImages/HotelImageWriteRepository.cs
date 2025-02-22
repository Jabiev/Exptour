using Exptour.Application.Abstract.Repositories.HotelImages;
using Exptour.Domain.Entities;
using Exptour.Persistence.Contexts;

namespace Exptour.Persistence.Concrete.Repositories.HotelImages;

public class HotelImageWriteRepository : WriteRepository<HotelImage>, IHotelImageWriteRepository
{
    public HotelImageWriteRepository(TourismManagementDbContext tourismManagementDbContext) : base(tourismManagementDbContext)
    {
    }
}

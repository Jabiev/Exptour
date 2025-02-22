using Exptour.Application.Abstract.Repositories.RoomImages;
using Exptour.Domain.Entities;
using Exptour.Persistence.Contexts;

namespace Exptour.Persistence.Concrete.Repositories.RoomImages;

public class RoomImageWriteRepository : WriteRepository<RoomImage>, IRoomImageWriteRepository
{
    public RoomImageWriteRepository(TourismManagementDbContext tourismManagementDbContext) : base(tourismManagementDbContext)
    {
    }
}

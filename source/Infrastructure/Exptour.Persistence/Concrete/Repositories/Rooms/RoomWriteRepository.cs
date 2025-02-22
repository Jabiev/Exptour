using Exptour.Application.Abstract.Repositories.RoomImages;
using Exptour.Application.Abstract.Repositories.Rooms;
using Exptour.Domain.Entities;
using Exptour.Persistence.Contexts;

namespace Exptour.Persistence.Concrete.Repositories.Rooms;

public class RoomWriteRepository : WriteRepository<Room>, IRoomWriteRepository
{
    public RoomWriteRepository(TourismManagementDbContext tourismManagementDbContext) : base(tourismManagementDbContext)
    {
    }
}

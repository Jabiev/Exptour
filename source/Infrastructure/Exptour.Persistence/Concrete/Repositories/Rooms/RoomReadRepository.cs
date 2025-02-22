using Exptour.Application.Abstract.Repositories.RoomImages;
using Exptour.Application.Abstract.Repositories.Rooms;
using Exptour.Domain.Entities;
using Exptour.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Exptour.Persistence.Concrete.Repositories.Rooms;

public class RoomReadRepository : ReadRepository<Room>, IRoomReadRepository
{
    public RoomReadRepository(TourismManagementDbContext tourismManagementDbContext) : base(tourismManagementDbContext)
    {
    }

    public IQueryable<Room> GetAvailableRooms(Guid hotelId, DateTime start, DateTime end)
        => Where(r => r.HotelId == hotelId
            && !_tourismManagementDbContext.Bookings
            .Any(b => b.StartDate <= end && b.EndDate >= start));
}

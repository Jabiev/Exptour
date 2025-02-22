using Exptour.Domain.Entities;

namespace Exptour.Application.Abstract.Repositories.Rooms;

public interface IRoomReadRepository : IReadRepository<Room>
{
    IQueryable<Room> GetAvailableRooms(Guid hotelId, DateTime start, DateTime end);
}

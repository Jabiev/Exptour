using Exptour.Application.Abstract.Repositories.Hotels;
using Exptour.Domain.Entities;
using Exptour.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Exptour.Persistence.Concrete.Repositories.Hotels;

public class HotelReadRepository : ReadRepository<Hotel>, IHotelReadRepository
{
    public HotelReadRepository(TourismManagementDbContext tourismManagementDbContext) : base(tourismManagementDbContext)
    {
    }

    public IQueryable<Hotel> GetHotelsByCity(string cityName)
        => Where(h => h.City.Name == cityName);

    public IQueryable<Hotel> GetHotelWithRooms(Guid hotelId)
        => Where(h => h.Id == hotelId)
            .Include(h => h.Rooms);
}

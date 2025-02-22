using Exptour.Domain.Entities;

namespace Exptour.Application.Abstract.Repositories.Hotels;

public interface IHotelReadRepository : IReadRepository<Hotel>
{
    IQueryable<Hotel> GetHotelsByCity(string cityName);
    IQueryable<Hotel> GetHotelWithRooms(Guid hotelId);
}

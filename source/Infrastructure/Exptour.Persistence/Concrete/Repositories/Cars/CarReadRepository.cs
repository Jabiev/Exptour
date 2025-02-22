using Exptour.Application.Abstract.Repositories.Cars;
using Exptour.Domain.Entities;
using Exptour.Persistence.Contexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Exptour.Persistence.Concrete.Repositories.Cars;

public class CarReadRepository : ReadRepository<Car>, ICarReadRepository
{
    public CarReadRepository(TourismManagementDbContext tourismManagementDbContext) : base(tourismManagementDbContext)
    {
    }

    public IQueryable<Car> GetAvailableCars(DateTime start, DateTime end)
        => Where(car => !car.Bookings.Any(booking => booking.StartDate <= end
                                                        && booking.EndDate >= start));

    public IQueryable<Car> GetCarsWithImages()
        => Table.Include(car => car.CarImages);
}

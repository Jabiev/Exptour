using Exptour.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Exptour.Application.Abstract.Repositories.Cars;

public interface ICarReadRepository : IReadRepository<Car>
{
    IQueryable<Car> GetAvailableCars(DateTime start, DateTime end);
    IQueryable<Car> GetCarsWithImages();
}

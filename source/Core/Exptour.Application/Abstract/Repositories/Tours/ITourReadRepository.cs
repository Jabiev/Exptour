using Exptour.Domain.Entities;

namespace Exptour.Application.Abstract.Repositories.Tours;

public interface ITourReadRepository : IReadRepository<Tour>
{
    IQueryable<Tour> GetUpcomingTours();

    IQueryable<Tour> GetToursByCity(Guid cityId);

    IQueryable<Tour> GetToursByCategory(Guid categoryId);

    IQueryable<Tour> GetToursByCategory(string categoryName);

    IQueryable<Tour> GetTourWithImages(Guid tourId);
}

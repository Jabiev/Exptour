using Exptour.Application.Abstract.Repositories.Tours;
using Exptour.Common.Helpers;
using Exptour.Domain.Entities;
using Exptour.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Exptour.Persistence.Concrete.Repositories.Tours;

public class TourReadRepository : ReadRepository<Tour>, ITourReadRepository
{
    public TourReadRepository(TourismManagementDbContext tourismManagementDbContext) : base(tourismManagementDbContext)
    {
    }

    public IQueryable<Tour> GetUpcomingTours()
        => Where(x => x.StartDate >= DateTime.UtcNow.ToUAE());

    public IQueryable<Tour> GetToursByCity(Guid cityId)
        => Where(x => x.CityId == cityId);

    public IQueryable<Tour> GetToursByCategory(Guid categoryId)
        => Where(x => x.Categories.Any(c => c.Id == categoryId));

    public IQueryable<Tour> GetToursByCategory(string categoryName)
        => Where(x => x.Categories.Any(c => c.NameEN == categoryName || c.NameAR == categoryName));

    public IQueryable<Tour> GetTourWithImages(Guid tourId)
        => Where(x => x.Id == tourId)
            .Include(x => x.TourImages);
}

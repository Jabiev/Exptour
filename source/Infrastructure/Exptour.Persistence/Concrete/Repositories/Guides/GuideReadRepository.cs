using Exptour.Application.Abstract.Repositories.Guides;
using Exptour.Domain.Entities;
using Exptour.Persistence.Contexts;

namespace Exptour.Persistence.Concrete.Repositories.Guides;

public class GuideReadRepository : ReadRepository<Guide>, IGuideReadRepository
{
    public GuideReadRepository(TourismManagementDbContext tourismManagementDbContext) : base(tourismManagementDbContext)
    {
    }

    public IQueryable<Guide> GetAvailableGuides(DateTime start, DateTime end, string language)
        => Where(g => g.Availabilities
            .Any(a => a.StartDate <= end && a.EndDate >= start)
            && g.Languages.Any(l => l.NameEN == language || l.NameAR == language));
}

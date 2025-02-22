using Exptour.Application.Abstract.Repositories.GuideAvailabilities;
using Exptour.Domain.Entities;
using Exptour.Persistence.Contexts;

namespace Exptour.Persistence.Concrete.Repositories.GuideAvailabilities;

public class GuideAvailabilityReadRepository : ReadRepository<GuideAvailability>, IGuideAvailabilityReadRepository
{
    public GuideAvailabilityReadRepository(TourismManagementDbContext tourismManagementDbContext) : base(tourismManagementDbContext)
    {
    }
}

using Exptour.Application.Abstract.Repositories.GuideAvailabilities;
using Exptour.Domain.Entities;
using Exptour.Persistence.Contexts;

namespace Exptour.Persistence.Concrete.Repositories.GuideAvailabilities;

public class GuideAvailabilityWriteRepository : WriteRepository<GuideAvailability>, IGuideAvailabilityWriteRepository
{
    public GuideAvailabilityWriteRepository(TourismManagementDbContext tourismManagementDbContext) : base(tourismManagementDbContext)
    {
    }
}

using Exptour.Application.Abstract.Repositories.GuideSchedules;
using Exptour.Domain.Entities;
using Exptour.Persistence.Contexts;

namespace Exptour.Persistence.Concrete.Repositories.GuideSchedules;

public class GuideScheduleReadRepository : ReadRepository<GuideSchedule>, IGuideScheduleReadRepository
{
    public GuideScheduleReadRepository(TourismManagementDbContext tourismManagementDbContext) : base(tourismManagementDbContext)
    {
    }
}

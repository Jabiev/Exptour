using Exptour.Application.Abstract.Repositories.GuideSchedules;
using Exptour.Domain.Entities;
using Exptour.Persistence.Contexts;

namespace Exptour.Persistence.Concrete.Repositories.GuideSchedules;

public class GuideScheduleWriteRepository : WriteRepository<GuideSchedule>, IGuideScheduleWriteRepository
{
    public GuideScheduleWriteRepository(TourismManagementDbContext tourismManagementDbContext) : base(tourismManagementDbContext)
    {
    }
}

using Exptour.Application.Abstract.Repositories.Guides;
using Exptour.Domain.Entities;
using Exptour.Persistence.Contexts;

namespace Exptour.Persistence.Concrete.Repositories.Guides;

public class GuideWriteRepository : WriteRepository<Guide>, IGuideWriteRepository
{
    public GuideWriteRepository(TourismManagementDbContext tourismManagementDbContext) : base(tourismManagementDbContext)
    {
    }
}

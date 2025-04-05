using Exptour.Application.Abstract.Repositories.Guides;
using Exptour.Domain.Entities;
using Exptour.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Exptour.Persistence.Concrete.Repositories.Guides;

public class GuideReadRepository : ReadRepository<Guide>, IGuideReadRepository
{
    public GuideReadRepository(TourismManagementDbContext tourismManagementDbContext) : base(tourismManagementDbContext)
    {
    }
}

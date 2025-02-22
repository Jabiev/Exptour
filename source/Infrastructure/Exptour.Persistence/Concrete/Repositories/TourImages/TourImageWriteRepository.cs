using Exptour.Application.Abstract.Repositories.TourImages;
using Exptour.Domain.Entities;
using Exptour.Persistence.Contexts;

namespace Exptour.Persistence.Concrete.Repositories.TourImages;

public class TourImageWriteRepository : WriteRepository<TourImage>, ITourImageWriteRepository
{
    public TourImageWriteRepository(TourismManagementDbContext tourismManagementDbContext) : base(tourismManagementDbContext)
    {
    }
}

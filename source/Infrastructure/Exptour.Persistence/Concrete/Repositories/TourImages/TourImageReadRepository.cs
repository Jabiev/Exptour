using Exptour.Application.Abstract.Repositories.TourImages;
using Exptour.Domain.Entities;
using Exptour.Persistence.Contexts;

namespace Exptour.Persistence.Concrete.Repositories.TourImages;

public class TourImageReadRepository : ReadRepository<TourImage>, ITourImageReadRepository
{
    public TourImageReadRepository(TourismManagementDbContext tourismManagementDbContext) : base(tourismManagementDbContext)
    {
    }
}

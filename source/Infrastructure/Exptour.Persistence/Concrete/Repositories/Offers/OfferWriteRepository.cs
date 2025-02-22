using Exptour.Application.Abstract.Repositories.Offers;
using Exptour.Domain.Entities;
using Exptour.Persistence.Contexts;

namespace Exptour.Persistence.Concrete.Repositories.Offers;

public class OfferWriteRepository : WriteRepository<Offer>, IOfferWriteRepository
{
    public OfferWriteRepository(TourismManagementDbContext tourismManagementDbContext) : base(tourismManagementDbContext)
    {
    }
}

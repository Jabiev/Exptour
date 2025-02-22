using Exptour.Application.Abstract.Repositories.Offers;
using Exptour.Domain.Entities;
using Exptour.Persistence.Contexts;

namespace Exptour.Persistence.Concrete.Repositories.Offers;

public class OfferReadRepository : ReadRepository<Offer>, IOfferReadRepository
{
    public OfferReadRepository(TourismManagementDbContext tourismManagementDbContext) : base(tourismManagementDbContext)
    {
    }
}

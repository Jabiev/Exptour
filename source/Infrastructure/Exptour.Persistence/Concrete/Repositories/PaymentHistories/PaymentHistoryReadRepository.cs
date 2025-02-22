using Exptour.Application.Abstract.Repositories.PaymentHistories;
using Exptour.Domain.Entities;
using Exptour.Persistence.Contexts;

namespace Exptour.Persistence.Concrete.Repositories.PaymentHistories;

public class PaymentHistoryReadRepository : ReadRepository<PaymentHistory>, IPaymentHistoryReadRepository
{
    public PaymentHistoryReadRepository(TourismManagementDbContext tourismManagementDbContext) : base(tourismManagementDbContext)
    {
    }
}

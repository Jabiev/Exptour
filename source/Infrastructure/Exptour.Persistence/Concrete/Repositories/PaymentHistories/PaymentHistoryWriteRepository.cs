using Exptour.Application.Abstract.Repositories.PaymentHistories;
using Exptour.Domain.Entities;
using Exptour.Persistence.Contexts;

namespace Exptour.Persistence.Concrete.Repositories.PaymentHistories;

public class PaymentHistoryWriteRepository : WriteRepository<PaymentHistory>, IPaymentHistoryWriteRepository
{
    public PaymentHistoryWriteRepository(TourismManagementDbContext tourismManagementDbContext) : base(tourismManagementDbContext)
    {
    }
}

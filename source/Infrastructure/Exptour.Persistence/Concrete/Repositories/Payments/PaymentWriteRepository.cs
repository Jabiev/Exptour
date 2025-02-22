using Exptour.Application.Abstract.Repositories.Payments;
using Exptour.Domain.Entities;
using Exptour.Persistence.Contexts;

namespace Exptour.Persistence.Concrete.Repositories.Payments;

public class PaymentWriteRepository : WriteRepository<Payment>, IPaymentWriteRepository
{
    public PaymentWriteRepository(TourismManagementDbContext tourismManagementDbContext) : base(tourismManagementDbContext)
    {
    }
}

using Exptour.Domain.Entities;

namespace Exptour.Application.Abstract.Repositories.Payments;

public interface IPaymentReadRepository : IReadRepository<Payment>
{
    IQueryable<Payment> GetPaymentsForUser(Guid userId);
    decimal GetTotalRevenueForDateRange(DateTime startDate, DateTime endDate);
    decimal GetTotalRevenueForDateRange(DateTime date);
}

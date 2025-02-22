using Exptour.Application.Abstract.Repositories.Payments;
using Exptour.Domain;
using Exptour.Domain.Entities;
using Exptour.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Exptour.Persistence.Concrete.Repositories.Payments;

public class PaymentReadRepository : ReadRepository<Payment>, IPaymentReadRepository
{
    public PaymentReadRepository(TourismManagementDbContext tourismManagementDbContext) : base(tourismManagementDbContext)
    {
    }

    public IQueryable<Payment> GetPaymentsForUser(Guid userId)
        => Where(p => p.Booking.ApplicationUserId == userId)
            .Include(p => p.PaymentHistories);

    public decimal GetTotalRevenueForDateRange(DateTime startDate, DateTime endDate)
        => Where(p => p.ProcessedAt >= startDate && p.ProcessedAt <= endDate && p.Status == PaymentStatus.Paid)
            .Sum(p => p.Amount);

    public decimal GetTotalRevenueForDateRange(DateTime date)
        => Where(p => p.ProcessedAt == date && p.Status == PaymentStatus.Paid)
            .Sum(p => p.Amount);
}

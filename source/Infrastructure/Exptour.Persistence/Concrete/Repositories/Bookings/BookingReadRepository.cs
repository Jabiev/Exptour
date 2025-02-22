using Exptour.Application.Abstract.Repositories.Bookings;
using Exptour.Domain;
using Exptour.Domain.Entities;
using Exptour.Persistence.Contexts;

namespace Exptour.Persistence.Concrete.Repositories.Bookings;

public class BookingReadRepository : ReadRepository<Booking>, IBookingReadRepository
{
    public BookingReadRepository(TourismManagementDbContext tourismManagementDbContext) : base(tourismManagementDbContext)
    {
    }

    public IQueryable<Booking> GetActiveBookingsForUser(Guid userId)
        => Where(b => b.ApplicationUserId == userId
            && !b.IsDeleted);
    public IQueryable<Booking> GetPendingBookingsForUser(Guid userId)
        => Where(b => b.ApplicationUserId == userId
            && !b.IsDeleted
            && b.Status == BookingStatus.Pending);
    public IQueryable<Booking> GetConfirmedBookingsForUser(Guid userId)
        => Where(b => b.ApplicationUserId == userId
            && !b.IsDeleted
            && b.Status == BookingStatus.Confirmed);
    public IQueryable<Booking> GetCancelledBookingsForUser(Guid userId)
        => Where(b => b.ApplicationUserId == userId
            && !b.IsDeleted
            && b.Status == BookingStatus.Cancelled);

    public IQueryable<Booking> GetBookingsForDateRange(DateTime start, DateTime end)
        => Where(b => b.StartDate >= start
            && b.EndDate <= end
            && !b.IsDeleted);
}

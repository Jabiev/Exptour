using Exptour.Domain.Entities;

namespace Exptour.Application.Abstract.Repositories.Bookings;

public interface IBookingReadRepository : IReadRepository<Booking>
{
    IQueryable<Booking> GetActiveBookingsForUser(Guid userId);
    IQueryable<Booking> GetPendingBookingsForUser(Guid userId);
    IQueryable<Booking> GetConfirmedBookingsForUser(Guid userId);
    IQueryable<Booking> GetCancelledBookingsForUser(Guid userId);
    IQueryable<Booking> GetBookingsForDateRange(DateTime start, DateTime end);
}

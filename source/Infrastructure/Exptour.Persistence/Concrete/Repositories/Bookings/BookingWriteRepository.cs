using Exptour.Application.Abstract.Repositories.Bookings;
using Exptour.Domain.Entities;
using Exptour.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Exptour.Persistence.Concrete.Repositories.Bookings;

public class BookingWriteRepository : WriteRepository<Booking>, IBookingWriteRepository
{
    public BookingWriteRepository(TourismManagementDbContext tourismManagementDbContext) : base(tourismManagementDbContext)
    {
    }
}

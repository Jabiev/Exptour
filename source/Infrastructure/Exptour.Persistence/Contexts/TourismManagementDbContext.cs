using Exptour.Domain.Entities;
using Exptour.Persistence.Interceptors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Exptour.Persistence.Contexts;

public class TourismManagementDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
    private readonly AuditInterceptor _auditInterceptor;

    public TourismManagementDbContext(DbContextOptions<TourismManagementDbContext> options, AuditInterceptor auditInterceptor) : base(options)
    {
        _auditInterceptor = auditInterceptor;
    }

    public virtual DbSet<ApplicationUser> ApplicationUsers { get; set; } = null!;
    public virtual DbSet<Booking> Bookings { get; set; } = null!;
    public virtual DbSet<Car> Cars { get; set; } = null!;
    public virtual DbSet<CarAvailability> CarAvailabilities { get; set; } = null!;
    public virtual DbSet<CarBrand> CarBrands { get; set; } = null!;
    public virtual DbSet<CarImage> CarImages { get; set; } = null!;
    public virtual DbSet<CarModel> CarModels { get; set; } = null!;
    public virtual DbSet<Category> Categories { get; set; } = null!;
    public virtual DbSet<City> Cities { get; set; } = null!;
    public virtual DbSet<Country> Countries { get; set; } = null!;
    public virtual DbSet<Driver> Drivers { get; set; } = null!;
    public virtual DbSet<Guide> Guides { get; set; } = null!;
    public virtual DbSet<GuideAvailability> GuideAvailabilities { get; set; } = null!;
    public virtual DbSet<Hotel> Hotels { get; set; } = null!;
    public virtual DbSet<HotelImage> HotelImages { get; set; } = null!;
    public virtual DbSet<Language> Languages { get; set; } = null!;
    public virtual DbSet<Offer> Offers { get; set; } = null!;
    public virtual DbSet<Payment> Payments { get; set; } = null!;
    public virtual DbSet<PaymentHistory> PaymentHistories { get; set; } = null!;
    public virtual DbSet<Permission> Permissions { get; set; } = null!;
    public virtual DbSet<RolePermission> RolePermissions { get; set; } = null!;
    public virtual DbSet<Room> Rooms { get; set; } = null!;
    public virtual DbSet<RoomImage> RoomImages { get; set; } = null!;
    public virtual DbSet<Tour> Tours { get; set; } = null!;
    public virtual DbSet<TourImage> TourImages { get; set; } = null!;


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        optionsBuilder.AddInterceptors(_auditInterceptor);
    }
}

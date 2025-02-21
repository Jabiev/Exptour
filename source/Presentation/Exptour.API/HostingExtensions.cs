using Exptour.Domain.Entities;
using Exptour.Persistence;
using Exptour.Persistence.Contexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Exptour.API;

public static class HostingExtensions
{
    public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
    {
        #region Database

        builder.Services.AddDbContext<TourismManagementDbContext>(options =>
        {
            options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSQL"));
        });

        builder.Services.AddIdentity<ApplicationUser, IdentityRole<Guid>>()
            .AddDefaultTokenProviders()
            .AddEntityFrameworkStores<TourismManagementDbContext>();

        #endregion

        #region Persistence Services

        builder.Services.ConfigurePersistenceServices();

        #endregion

        return builder.Build();
    }
}

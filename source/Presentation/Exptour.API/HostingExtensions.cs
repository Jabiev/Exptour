using Exptour.Domain.Entities;
using Exptour.Infrastructure;
using Exptour.Persistence;
using Exptour.Persistence.Contexts;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

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

        #region Infrastructure Services

        builder.Services.ConfigureInfrastructureServices();

        #endregion

        #region JWT

        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    LifetimeValidator = (_, expire, _, _) => expire > DateTime.UtcNow,
                    ValidIssuer = builder.Configuration["JWTSettings:Issuer"],
                    ValidAudience = builder.Configuration["JWTSettings:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWTSettings:SecurityKey"])),
                    ClockSkew = TimeSpan.Zero
                };
            });

        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("OnlyAdmins", policy => policy.RequireRole("Admin"));
        });

        #endregion

        builder.Services.AddHttpContextAccessor();

        #region Google

        builder.Services.AddAuthentication()
            .AddGoogle(x =>
            {
                x.ClientId = builder.Configuration["GoogleSettings:ClientId"];
                x.ClientSecret = builder.Configuration["GoogleSettings:ClientSecret"];
            });

        #endregion

        return builder.Build();
    }
}

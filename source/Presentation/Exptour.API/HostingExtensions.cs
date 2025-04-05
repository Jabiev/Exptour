using Exptour.Application.BackgroundServices.Guide;
using Exptour.Domain.Entities;
using Exptour.Infrastructure;
using Exptour.Infrastructure.ElasticSearch;
using Exptour.Persistence;
using Exptour.Persistence.Contexts;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Nest;
using Quartz;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Text;
using System.Text.Json.Serialization;

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

        #region JWT & Authentication & Authorization

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
            options.AddPolicy("Guides", policy => policy.RequireRole("Guide"));
            options.AddPolicy("AdminsOrGuides", policy => policy.RequireRole("Admin", "Guide"));
            options.AddPolicy("AdminsOrTourismManagers", policy => policy.RequireRole("Admin", "Tourism Manager"));
            options.AddPolicy("AdminsOrGuidesOrTourismManagers", policy => policy.RequireRole("Admin", "Guide", "Tourism Manager"));

            options.AddPolicy("VerifiedEmailsOnly", policy => policy.RequireClaim("EmailVerified", "true"));
            options.AddPolicy("VIPUsers", policy => policy.RequireClaim("SubscriptionLevel", "VIP"));
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

        #region Swagger

        builder.Services.AddSwaggerDocument(configure =>
        {
            configure.PostProcess = (doc =>
            {
                doc.Info.Title = "Explore Tour Guide";
                doc.Info.Version = "1.0.0";
                doc.Info.Description = "Booking all you need | Cars, Guides, Hotels, Ready Packages and everything you need";
                doc.Info.Contact = new NSwag.OpenApiContact()
                {
                    Name = "E x p t o u r",
                    Url = "https://www.youtube.com/@iamjabiev",
                    Email = "jabieviam@gmail.com"
                };
            });
        });

        builder.Services.AddSwaggerGen(c =>
        {
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Please Enter like 'Bearer {token}' format",
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[] {}
                }
            });
        });

        builder.Services.AddSwaggerGen(options =>
        {
            options.SchemaFilter<EnumSchemaFilter>();
            //options.SchemaFilter<DateTimeSchemaFilter>();
        });

        #endregion

        builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

        #region Hosted Services (Background Services) & Health Checks

        builder.Services.AddHostedService<GuideAvailabilityBackgroundService>();
        builder.Services.AddHealthChecks()
            .AddCheck<GuideAvailabilityHealthCheck>("Guide Schedule Health Check");

        #endregion

        #region Quartz

        builder.Services.AddQuartz();
        builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

        #endregion

        #region Configure Json Options

        builder.Services.Configure<JsonOptions>(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });

        #endregion

        #region Elastic Search

        var elasticConfig = new ElasticSearchConfig();
        builder.Configuration.GetSection("ElasticSearch").Bind(elasticConfig);

        var settings = new ConnectionSettings(new Uri(elasticConfig.Uri))
            .BasicAuthentication(elasticConfig.Username, elasticConfig.Password)
            .DefaultIndex(elasticConfig.DefaultIndex);

        var client = new ElasticClient(settings);
        builder.Services.AddSingleton<IElasticClient>(client);

        #endregion

        #region RabbitMQ

        builder.Services.Configure<Application.Settings.RabbitMQ>(builder.Configuration.GetSection("RabbitMQ"));

        #endregion

        #region MediatR

        builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies()));

        #endregion

        #region Detect Language API

        builder.Services.Configure<Application.Settings.DetectLanguageAPI>(builder.Configuration.GetSection("DetectLanguageAPI"));

        #endregion

        return builder.Build();
    }
}

#region SchemaFilters

public class EnumSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (context.Type.IsEnum)
        {
            schema.Enum.Clear();
            foreach (var enumValue in Enum.GetValues(context.Type))
            {
                schema.Enum.Add(new OpenApiString(enumValue.ToString()));
            }
        }
    }
}

//public class DateTimeSchemaFilter : ISchemaFilter
//{
//    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
//    {
//        if (context.Type == typeof(DateTime) || context.Type == typeof(DateTime?))
//        {
//            schema.Format = "MM-dd-yyyy";
//            schema.Example = new Microsoft.OpenApi.Any.OpenApiString(DateTime.Now.ToString("MM-dd-yyyy HH:mm:ss"));
//        }
//    }
//}

#endregion

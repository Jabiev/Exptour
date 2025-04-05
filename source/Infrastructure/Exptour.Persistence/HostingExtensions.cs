using Exptour.Application.Abstract.Repositories.Bookings;
using Exptour.Application.Abstract.Repositories.CarAvailabilities;
using Exptour.Application.Abstract.Repositories.CarBrands;
using Exptour.Application.Abstract.Repositories.CarImages;
using Exptour.Application.Abstract.Repositories.CarModels;
using Exptour.Application.Abstract.Repositories.Cars;
using Exptour.Application.Abstract.Repositories.Categories;
using Exptour.Application.Abstract.Repositories.Cities;
using Exptour.Application.Abstract.Repositories.Countries;
using Exptour.Application.Abstract.Repositories.Drivers;
using Exptour.Application.Abstract.Repositories.EndpointRoles;
using Exptour.Application.Abstract.Repositories.Endpoints;
using Exptour.Application.Abstract.Repositories.Guides;
using Exptour.Application.Abstract.Repositories.GuideSchedules;
using Exptour.Application.Abstract.Repositories.HotelImages;
using Exptour.Application.Abstract.Repositories.Hotels;
using Exptour.Application.Abstract.Repositories.Languages;
using Exptour.Application.Abstract.Repositories.Menus;
using Exptour.Application.Abstract.Repositories.Offers;
using Exptour.Application.Abstract.Repositories.PaymentHistories;
using Exptour.Application.Abstract.Repositories.Payments;
using Exptour.Application.Abstract.Repositories.RoomImages;
using Exptour.Application.Abstract.Repositories.Rooms;
using Exptour.Application.Abstract.Repositories.TourImages;
using Exptour.Application.Abstract.Repositories.Tours;
using Exptour.Application.Abstract.Services;
using Exptour.Persistence.Concrete.Repositories.Bookings;
using Exptour.Persistence.Concrete.Repositories.CarAvailabilities;
using Exptour.Persistence.Concrete.Repositories.CarBrands;
using Exptour.Persistence.Concrete.Repositories.CarImages;
using Exptour.Persistence.Concrete.Repositories.CarModels;
using Exptour.Persistence.Concrete.Repositories.Cars;
using Exptour.Persistence.Concrete.Repositories.Categories;
using Exptour.Persistence.Concrete.Repositories.Cities;
using Exptour.Persistence.Concrete.Repositories.Countries;
using Exptour.Persistence.Concrete.Repositories.Drivers;
using Exptour.Persistence.Concrete.Repositories.EndpointRoles;
using Exptour.Persistence.Concrete.Repositories.Endpoints;
using Exptour.Persistence.Concrete.Repositories.Guides;
using Exptour.Persistence.Concrete.Repositories.GuideSchedules;
using Exptour.Persistence.Concrete.Repositories.HotelImages;
using Exptour.Persistence.Concrete.Repositories.Hotels;
using Exptour.Persistence.Concrete.Repositories.Languages;
using Exptour.Persistence.Concrete.Repositories.Menus;
using Exptour.Persistence.Concrete.Repositories.Offers;
using Exptour.Persistence.Concrete.Repositories.PaymentHistories;
using Exptour.Persistence.Concrete.Repositories.Payments;
using Exptour.Persistence.Concrete.Repositories.RoomImages;
using Exptour.Persistence.Concrete.Repositories.Rooms;
using Exptour.Persistence.Concrete.Repositories.TourImages;
using Exptour.Persistence.Concrete.Repositories.Tours;
using Exptour.Persistence.Concrete.Services;
using Exptour.Persistence.Interceptors;
using Microsoft.Extensions.DependencyInjection;

namespace Exptour.Persistence;

public static class HostingExtensions
{
    public static void ConfigurePersistenceServices(this IServiceCollection services)
    {
        #region Interceptors

        services.AddScoped<AuditInterceptor>();

        #endregion

        #region Repositories

        services.AddScoped<IBookingReadRepository, BookingReadRepository>();
        services.AddScoped<IBookingWriteRepository, BookingWriteRepository>();
        services.AddScoped<ICarAvailabilityReadRepository, CarAvailabilityReadRepository>();
        services.AddScoped<ICarAvailabilityWriteRepository, CarAvailabilityWriteRepository>();
        services.AddScoped<ICarBrandReadRepository, CarBrandReadRepository>();
        services.AddScoped<ICarBrandWriteRepository, CarBrandWriteRepository>();
        services.AddScoped<ICarImageReadRepository, CarImageReadRepository>();
        services.AddScoped<ICarImageWriteRepository, CarImageWriteRepository>();
        services.AddScoped<ICarModelReadRepository, CarModelReadRepository>();
        services.AddScoped<ICarModelWriteRepository, CarModelWriteRepository>();
        services.AddScoped<ICarReadRepository, CarReadRepository>();
        services.AddScoped<ICarWriteRepository, CarWriteRepository>();
        services.AddScoped<ICategoryReadRepository, CategoryReadRepository>();
        services.AddScoped<ICategoryWriteRepository, CategoryWriteRepository>();
        services.AddScoped<ICityReadRepository, CityReadRepository>();
        services.AddScoped<ICityWriteRepository, CityWriteRepository>();
        services.AddScoped<ICountryReadRepository, CountryReadRepository>();
        services.AddScoped<ICountryWriteRepository, CountryWriteRepository>();
        services.AddScoped<IDriverReadRepository, DriverReadRepository>();
        services.AddScoped<IDriverWriteRepository, DriverWriteRepository>();
        services.AddScoped<IEndpointRoleReadRepository, EndpointRoleReadRepository>();
        services.AddScoped<IEndpointRoleWriteRepository, EndpointRoleWriteRepository>();
        services.AddScoped<IEndpointReadRepository, EndpointReadRepository>();
        services.AddScoped<IEndpointWriteRepository, EndpointWriteRepository>();
        services.AddScoped<IGuideScheduleReadRepository, GuideScheduleReadRepository>();
        services.AddScoped<IGuideScheduleWriteRepository, GuideScheduleWriteRepository>();
        services.AddScoped<IGuideReadRepository, GuideReadRepository>();
        services.AddScoped<IGuideWriteRepository, GuideWriteRepository>();
        services.AddScoped<IHotelImageReadRepository, HotelImageReadRepository>();
        services.AddScoped<IHotelImageWriteRepository, HotelImageWriteRepository>();
        services.AddScoped<IHotelReadRepository, HotelReadRepository>();
        services.AddScoped<IHotelWriteRepository, HotelWriteRepository>();
        services.AddScoped<ILanguageReadRepository, LanguageReadRepository>();
        services.AddScoped<IMenuReadRepository, MenuReadRepository>();
        services.AddScoped<IMenuWriteRepository, MenuWriteRepository>();
        services.AddScoped<ILanguageWriteRepository, LanguageWriteRepository>();
        services.AddScoped<IOfferReadRepository, OfferReadRepository>();
        services.AddScoped<IOfferWriteRepository, OfferWriteRepository>();
        services.AddScoped<IPaymentHistoryReadRepository, PaymentHistoryReadRepository>();
        services.AddScoped<IPaymentHistoryWriteRepository, PaymentHistoryWriteRepository>();
        services.AddScoped<IPaymentReadRepository, PaymentReadRepository>();
        services.AddScoped<IPaymentWriteRepository, PaymentWriteRepository>();
        services.AddScoped<IRoomImageReadRepository, RoomImageReadRepository>();
        services.AddScoped<IRoomImageWriteRepository, RoomImageWriteRepository>();
        services.AddScoped<IRoomReadRepository, RoomReadRepository>();
        services.AddScoped<IRoomWriteRepository, RoomWriteRepository>();
        services.AddScoped<ITourImageReadRepository, TourImageReadRepository>();
        services.AddScoped<ITourImageWriteRepository, TourImageWriteRepository>();
        services.AddScoped<ITourReadRepository, TourReadRepository>();
        services.AddScoped<ITourWriteRepository, TourWriteRepository>();

        #endregion

        #region Services

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<IAuthorizationEndpointService, AuthorizationEndpointService>();
        services.AddScoped<ICountryService, CountryService>();
        services.AddScoped<ICityService, CityService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<ILanguageService, LanguageService>();
        services.AddScoped<IGuideService, GuideService>();

        #endregion
    }
}

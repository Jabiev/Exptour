using AutoMapper;
using Exptour.Application.DTOs.City;
using Exptour.Domain.Entities;

namespace Exptour.Application.Mappings;

public class CityProfile : Profile
{
    public CityProfile()
    {
        CreateMap<City, CityDTO>().ReverseMap();
        CreateMap<City, CityResponse>().ReverseMap();
    }
}

using AutoMapper;
using Exptour.Application.DTOs.Country;
using Exptour.Domain.Entities;

namespace Exptour.Application.Mappings;

public class CountryProfile : Profile
{
    public CountryProfile()
    {
        CreateMap<Country, CountryDTO>().ReverseMap();
        CreateMap<Country, CountryResponse>().ReverseMap();
    }
}

using AutoMapper;
using Exptour.Application.DTOs.Language;
using Exptour.Common;

namespace Exptour.Application.Mappings;

public class LanguageProfile : Profile
{
    public LanguageProfile()
    {
        CreateMap<Language, CreateLanguageDTO>().ReverseMap();
        CreateMap<Language, LanguageResponse>().ReverseMap();
    }
}

using AutoMapper;
using Exptour.Application.DTOs.Guide;
using Exptour.Domain.Entities;

namespace Exptour.Application.Mappings;

public class GuideProfile : Profile
{
    public GuideProfile()
    {
        CreateMap<Guide, CreateGuideDTO>().ReverseMap();
        CreateMap<Guide, UpdateGuideDTO>().ReverseMap();
        CreateMap<Guide, GuideResponse>().ReverseMap();
    }
}

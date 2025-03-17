using AutoMapper;
using Exptour.Application.DTOs.Category;
using Exptour.Domain.Entities;

namespace Exptour.Application.Mappings;

public class CategoryProfile : Profile
{
    public CategoryProfile()
    {
        CreateMap<Category, CategoryDTO>().ReverseMap();
        CreateMap<Category, CategoryResponse>().ReverseMap();
    }
}

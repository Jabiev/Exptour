using AutoMapper;
using Exptour.Application.Abstract.Repositories.Categories;
using Exptour.Application.Abstract.Services;
using Exptour.Application.DTOs.Category;
using Exptour.Application.Validators.Category;
using Exptour.Common.Helpers;
using Exptour.Common.Infrastructure.Services;
using Exptour.Common.Shared;
using Exptour.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Text;

namespace Exptour.Persistence.Concrete.Services;

public class CategoryService : BaseService, ICategoryService
{
    private readonly IMapper _mapper;
    private readonly ICategoryReadRepository _categoryReadRepository;
    private readonly ICategoryWriteRepository _categoryWriteRepository;

    public CategoryService(IHttpContextAccessor httpContextAccessor,
        IConfiguration configuration,
        IMapper mapper,
        ICategoryReadRepository categoryReadRepository,
        ICategoryWriteRepository categoryWriteRepository) : base(httpContextAccessor, configuration)
    {
        _mapper = mapper;
        _categoryReadRepository = categoryReadRepository;
        _categoryWriteRepository = categoryWriteRepository;
    }

    public async Task<APIResponse<CategoryResponse>> CreateAsync(CategoryDTO categoryDTO)
    {
        var response = new APIResponse<CategoryResponse>();

        var currentLanguage = GetCurrentLanguage();

        CategoryDTOValidator validations = new();

        var result = await validations.ValidateAsync(categoryDTO);
        if (!result.IsValid)
        {
            StringBuilder stringBuilder = new();
            foreach (var error in result.Errors)
                stringBuilder.AppendLine(error.ErrorMessage);
            response.ResponseCode = HttpStatusCode.BadRequest;
            response.Message = stringBuilder.ToString();
            response.State = GetMessageByLocalization("InvalidRequest").state;
            return response;
        }

        if (await _categoryReadRepository.FirstOrDefaultAsync(c => c.NameEN == categoryDTO.NameEN && !c.IsDeleted, false) is not null)
        {
            var msgCategoryAlreadyExists = GetMessageByLocalization("AlreadyExists");
            response.ResponseCode = HttpStatusCode.BadRequest;
            response.Message = msgCategoryAlreadyExists.message;
            response.State = msgCategoryAlreadyExists.state;
            return response;
        }

        Category category = new()
        {
            NameEN = categoryDTO.NameEN,
            NameAR = categoryDTO.NameAR,
            Description = categoryDTO.Description,
        };

        await _categoryWriteRepository.AddAsync(category);
        await _categoryWriteRepository.SaveChangesAsync();

        response.Payload = new CategoryResponse(category.Id, Helper.GetByLanguage(category.NameEN, category.NameAR, currentLanguage), category.Description);
        return response;
    }

    public async Task<APIResponse<CategoryResponse>> DeleteAsync(string id)
    {
        var response = new APIResponse<CategoryResponse>();

        var currentLanguage = GetCurrentLanguage();

        if (!id.IsValidGuid())
        {
            var msgInvalidRequest = GetMessageByLocalization("InvalidRequest");
            response.ResponseCode = HttpStatusCode.BadRequest;
            response.Message = msgInvalidRequest.message;
            response.State = msgInvalidRequest.state;
            return response;
        }

        var category = await _categoryReadRepository.FirstOrDefaultAsync(c => c.Id == Guid.Parse(id) && !c.IsDeleted, false);
        if (category is null)
        {
            var msgNotFound = GetMessageByLocalization("NotFound");
            response.Message = msgNotFound.message;
            response.ResponseCode = HttpStatusCode.NotFound;
            response.State = msgNotFound.state;
            return response;
        }

        category.IsDeleted = true;
        _categoryWriteRepository.Update(category);
        await _categoryWriteRepository.SaveChangesAsync();

        response.Payload = new CategoryResponse(category.Id, Helper.GetByLanguage(category.NameEN, category.NameAR, currentLanguage), category.Description);
        return response;
    }

    public async Task<APIResponse<Pagination<CategoryResponse>>> GetAllAsync(int pageNumber = 1, int take = 10, bool isPaginated = false)
    {
        var response = new APIResponse<Pagination<CategoryResponse>>();

        var currentLanguage = GetCurrentLanguage();

        if (pageNumber < 1 || take < 1)
        {
            var msgInvalidRequest = GetMessageByLocalization("InvalidRequest");
            response.ResponseCode = HttpStatusCode.BadRequest;
            response.Message = msgInvalidRequest.message;
            response.State = msgInvalidRequest.state;
            return response;
        }

        var categories = isPaginated
            ? _categoryReadRepository.Table
            .Where(c => !c.IsDeleted)
                .Skip((pageNumber - 1) * take)
                .Take(take)
            : _categoryReadRepository.Table
            .Where(c => !c.IsDeleted);

        var categoriesCount = await categories.CountAsync();

        response.Payload = new Pagination<CategoryResponse>()
        {
            Items = await categories.Select(x => new CategoryResponse(x.Id, Helper.GetByLanguage(x.NameEN, x.NameAR, currentLanguage), x.Description)).ToListAsync(),
            PageIndex = pageNumber,
            PageSize = isPaginated ? take : categoriesCount,
            TotalCount = categoriesCount,
            TotalPage = isPaginated ? (int)Math.Ceiling((double)categoriesCount / take) : 1
        };
        return response;
    }

    public async Task<APIResponse<CategoryResponse>> GetByIdAsync(string id)
    {
        var response = new APIResponse<CategoryResponse>();

        var currentLanguage = GetCurrentLanguage();

        if (!id.IsValidGuid())
        {
            var msgInvalidRequest = GetMessageByLocalization("InvalidRequest");
            response.ResponseCode = HttpStatusCode.BadRequest;
            response.Message = msgInvalidRequest.message;
            response.State = msgInvalidRequest.state;
            return response;
        }

        var category = await _categoryReadRepository.FirstOrDefaultAsync(c => c.Id == Guid.Parse(id) && !c.IsDeleted, false);
        if (category is null)
        {
            var msgNotFound = GetMessageByLocalization("NotFound");
            response.Message = msgNotFound.message;
            response.ResponseCode = HttpStatusCode.NotFound;
            response.State = msgNotFound.state;
            return response;
        }

        response.Payload = new CategoryResponse(category.Id, Helper.GetByLanguage(category.NameEN, category.NameAR, currentLanguage), category.Description);
        return response;
    }
}

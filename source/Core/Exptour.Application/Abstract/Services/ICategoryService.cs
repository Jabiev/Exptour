using Exptour.Application.DTOs.Category;
using Exptour.Common.Shared;

namespace Exptour.Application.Abstract.Services;

public interface ICategoryService
{
    Task<APIResponse<CategoryResponse>> CreateAsync(CategoryDTO categoryDTO);
    Task<APIResponse<Pagination<CategoryResponse>>> GetAllAsync(int pageNumber = 1, int take = 10, bool isPaginated = false);
    Task<APIResponse<CategoryResponse>> GetByIdAsync(string id);
    Task<APIResponse<CategoryResponse>> DeleteAsync(string id);
}

using Exptour.Application.DTOs.Language;
using Exptour.Common.Shared;

namespace Exptour.Application.Abstract.Services;

public interface ILanguageService
{
    Task<APIResponse<Pagination<LanguageResponse>>> GetAllAsync(int pageNumber = 1, int take = 10, bool isPaginated = false);
    Task<APIResponse<LanguageResponse>> GetByIdAsync(string id);
    Task<APIResponse<LanguageResponse>> CreateAsync(CreateLanguageDTO createLanguageDto);
    Task<APIResponse<LanguageResponse>> DeleteAsync(string id);
}

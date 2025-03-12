using Exptour.Application.DTOs.Country;
using Exptour.Common.Shared;

namespace Exptour.Application.Abstract.Services;

public interface ICountryService
{
    Task<APIResponse<Pagination<CountryResponse>>> GetAllAsync(int pageNumber = 1, int take = 10, bool isPaginated = false);
    Task<APIResponse<CountryResponse>> GetByIdAsync(string id);
    Task<APIResponse<Pagination<CountryResponse>>> GetByNameAsync(string clue, int pageNumber = 1, int take = 10, bool isPaginated = false);
    Task<APIResponse<CountryResponse>> CreateAsync(CountryDTO countryDTO);
    Task<APIResponse<CountryResponse>> UpdateAsync(string id, CountryDTO countryDTO);
    Task<APIResponse<CountryResponse>> DeleteAsync(string id);
}

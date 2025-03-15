using Exptour.Application.DTOs.City;
using Exptour.Common.Shared;

namespace Exptour.Application.Abstract.Services;

public interface ICityService
{
    Task<APIResponse<Pagination<CityResponse>>> GetAllAsync(int pageNumber = 1, int take = 10, bool isPaginated = false);
    Task<APIResponse<CityResponse>> GetByIdAsync(string id);
    Task<APIResponse<Pagination<CityResponse>>> GetByNameAsync(string clue, int pageNumber = 1, int take = 10, bool isPaginated = false);
    Task<APIResponse<Pagination<CityResponse>>> GetByCountryIdAsync(string countryId, int pageNumber = 1, int take = 10, bool isPaginated = false);
    Task<APIResponse<CityResponse>> CreateAsync(CityDTO cityDto);
    Task<APIResponse<CityResponse>> UpdateAsync(string id, UpdateCityDTO updateCityDto);
    Task<APIResponse<CityResponse>> DeleteAsync(string id);
}

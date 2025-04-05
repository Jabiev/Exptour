using Exptour.Application.DTOs.Guide;
using Exptour.Common.Shared;

namespace Exptour.Application.Abstract.Services;

public interface IGuideService
{
    Task<APIResponse<SetScheduleResponse>> AssignScheduleAsync(string id, DateTime startDate, DateTime endDate);
    Task<APIResponse<AssignLanguageResponse>> AssignLanguageAsync(List<string> languageIds);
    Task<APIResponse<GuideStatisticsResponse>> GetGuidesStatisticsAsync();
    Task<APIResponse<Pagination<GetGuideSchedulesResponse>>> GetGuideSchedulesAsync(int pageNumber = 1, int take = 10, bool isPaginated = false);
    Task<APIResponse<Pagination<GetAllByLanguageResponse>>> GetAllByLanguageAsync(string language, int pageNumber = 1, int take = 10, bool isPaginated = false);
    Task<APIResponse<Pagination<GetAllResponse>>> GetAllAsync(int pageNumber = 1, int take = 10, bool isPaginated = false);
    Task<APIResponse<CreateGuideResponse>> CreateAsync(CreateGuideDTO createGuideDTO);
    Task<APIResponse<GuideResponse>> GetByIdAsync(string id);
    Task<APIResponse<GuideResponse>> SetOnLeaveAsync(string id, int day);
    Task<APIResponse<GuideResponse>> SetRetireAsync(string id);
    Task<APIResponse<GuideResponse>> DeleteAsync(string id);
    Task<APIResponse<GuideResponse>> UpdateAsync(UpdateGuideDTO updateGuideDTO);
    Task<APIResponse<SetScheduleResponse>> UpdateScheduleAsync(string id, string scheduleId, DateTime newStartDate, DateTime newEndDate);
    Task<APIResponse<GuideResponse>> UnassignLanguageAsync(List<string> languageIds);
}

namespace Exptour.Application.DTOs.Guide;

public record GetAllResponse(Guid Id, string FullName, string Email, string Gender, bool IsAvailable, bool IsActive, List<GuideLanguage> Languages, List<GuideSchedule> Schedules, int ParticipatedToursCount, int UpcomingToursCount);
public record GuideLanguage(Guid Id, string Name);
public record GuideSchedule(Guid ScheduleId, DateTime StartDate, DateTime EndDate);

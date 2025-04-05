namespace Exptour.Application.DTOs.Guide;

public record GuideResponse(Guid Id, string FullName, string Email, string Gender, List<string> Languages, bool IsAvailable, bool IsActive, string? LastSchedule, int ParticipatedToursCount, int UpcomingToursCount);

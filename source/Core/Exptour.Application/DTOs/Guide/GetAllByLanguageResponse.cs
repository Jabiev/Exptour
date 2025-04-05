namespace Exptour.Application.DTOs.Guide;

public record GetAllByLanguageResponse(Guid Id, string FullName, string Email, string Gender, bool IsAvailable, bool IsActive, List<GuideLanguage> Languages, int ParticipatedToursCount, int UpcomingToursCount);

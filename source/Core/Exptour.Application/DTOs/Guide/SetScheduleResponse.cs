namespace Exptour.Application.DTOs.Guide;

public record SetScheduleResponse(Guid Id, bool IsAvailable, string StartDate, string EndDate);

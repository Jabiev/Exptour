namespace Exptour.Application.DTOs.Guide;

public record GetGuideSchedulesResponse(Guid ScheduleId, DateTime StartDate, DateTime EndDate, string TourThumbnail, string TourName);

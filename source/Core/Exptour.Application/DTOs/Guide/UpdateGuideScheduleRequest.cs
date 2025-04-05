namespace Exptour.Application.DTOs.Guide;

public record UpdateGuideScheduleRequest(string ScheduleId, DateTime NewStartDate, DateTime NewEndDate);

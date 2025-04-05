namespace Exptour.Application.DTOs.Guide;

public record AssignLanguageResponse(Guid Id, string FullName, string Email, List<string> Languages);

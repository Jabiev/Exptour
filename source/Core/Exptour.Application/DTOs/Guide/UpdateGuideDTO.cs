namespace Exptour.Application.DTOs.Guide;

public record UpdateGuideDTO(string? NewFullName, string? NewEmail, List<string>? NewLanguageIds);

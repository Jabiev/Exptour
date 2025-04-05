namespace Exptour.Application.DTOs.User;

public record UpdateProfileDTO(string? UserName,
    string? LastName,
    /// <summary>
    /// Nationality by AcceptLanguage, such as en or ar
    /// </summary>
    string? Nationality,
    string? Email,
    string? PhoneNumber);

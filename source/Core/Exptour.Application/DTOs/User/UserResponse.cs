namespace Exptour.Application.DTOs.User;

public record UserResponse(Guid Id, string Email, string FullName, List<string>? Roles);

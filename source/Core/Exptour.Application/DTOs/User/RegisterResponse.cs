namespace Exptour.Application.DTOs.User;

public record RegisterResponse(Guid UserId, string UserName, string Email);

namespace Exptour.Application.DTOs.User;

public record UpdatePasswordDTO(string UserId, string ResetToken, string NewPassword);

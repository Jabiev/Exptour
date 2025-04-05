namespace Exptour.Application.DTOs.User;

public record UpdatePasswordDTO(string ResetToken, string NewPassword);

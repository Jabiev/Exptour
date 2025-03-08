namespace Exptour.Application.DTOs.User;

public record UpdateProfileDTO(string UserId, string UserName, string LastName, string Email, string PhoneNumber);

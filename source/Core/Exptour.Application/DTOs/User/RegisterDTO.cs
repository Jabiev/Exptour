using Exptour.Domain;

namespace Exptour.Application.DTOs.User;

public record RegisterDTO(string? LastName, string UserName, string Email, string Password, Gender? Gender);

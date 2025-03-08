namespace Exptour.Application.DTOs.AuthEndpoints;

public record AssignRoleRequest(string[] Roles, string Menu, string Code);

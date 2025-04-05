using MediatR;

namespace Exptour.Domain.Events;

public record UserEvent(Guid Id, string Email, string FullName, List<string>? Roles) : INotification;

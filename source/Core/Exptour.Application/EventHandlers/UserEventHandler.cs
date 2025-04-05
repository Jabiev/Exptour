using Exptour.Application.Abstract.Services;
using Exptour.Domain.Events;
using MediatR;

namespace Exptour.Application.EventHandlers;

public class UserEventHandler : INotificationHandler<UserEvent>
{
    private readonly IMessageQueueService _messageQueueService;

    public UserEventHandler(IMessageQueueService messageQueueService)
    {
        _messageQueueService = messageQueueService;
    }

    public Task Handle(UserEvent notification,
        CancellationToken cancellationToken)
    {
        _messageQueueService.Publish(notification);
        return Task.CompletedTask;
    }
}

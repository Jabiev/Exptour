namespace Exptour.Application.Abstract.Services;

public interface IMessageQueueService
{
    Task Publish<T>(T message);
}

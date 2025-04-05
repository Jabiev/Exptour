using Exptour.Application.Abstract.Services;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace Exptour.Infrastructure.Messaging;

public class RabbitMqService : IMessageQueueService
{
    private readonly Application.Settings.RabbitMQ _rabbitMQSettings;
    private readonly string _queueName = "UserUpdatedQueue";

    public RabbitMqService(IOptions<Application.Settings.RabbitMQ> options)
    {
        _rabbitMQSettings = options.Value;
    }

    public async Task Publish<T>(T message)
    {
        var factory = new ConnectionFactory()
        {
            HostName = _rabbitMQSettings.HostName,
            UserName = _rabbitMQSettings.UserName,
            Password = _rabbitMQSettings.Password,
            VirtualHost = _rabbitMQSettings.VirtualHost,
            Port = _rabbitMQSettings.Port,
            Ssl = new SslOption { Enabled = _rabbitMQSettings.UseSSL }
        };

        await using var connection = await factory.CreateConnectionAsync();
        await using var channel = await connection.CreateChannelAsync();

        await channel.QueueDeclareAsync(queue: _queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

        var jsonMessage = JsonConvert.SerializeObject(message);
        var body = Encoding.UTF8.GetBytes(jsonMessage);

        await channel.BasicPublishAsync(exchange: "", routingKey: _queueName, body: body);
    }
}

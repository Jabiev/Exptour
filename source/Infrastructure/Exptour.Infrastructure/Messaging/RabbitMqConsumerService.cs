using Exptour.Domain.Events;
using Exptour.Infrastructure.ElasticSearch.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Exptour.Infrastructure.Messaging;

public class RabbitMqConsumerService : BackgroundService
{
    private readonly string _queueName = "UserUpdatedQueue";
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly Application.Settings.RabbitMQ _rabbitMQSettings;
    private readonly ILogger<RabbitMqConsumerService> _logger;
    private IConnection? _connection;
    private IChannel? _channel;

    public RabbitMqConsumerService(IServiceScopeFactory serviceScopeFactory,
        IOptions<Application.Settings.RabbitMQ> options,
        ILogger<RabbitMqConsumerService> logger)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _rabbitMQSettings = options.Value;
        _logger = logger;
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
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
        _connection = await factory.CreateConnectionAsync();
        _channel = await _connection.CreateChannelAsync();

        await _channel.QueueDeclareAsync(queue: _queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

        await base.StartAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (_channel is null)
            return;

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += async (model, ea) =>
        {
            try
            {
                var body = ea.Body.ToArray();
                var jsonMessage = Encoding.UTF8.GetString(body);
                var userEvent = JsonConvert.DeserializeObject<UserEvent>(jsonMessage);

                if (userEvent is null)
                    return;

                using var scope = _serviceScopeFactory.CreateScope();
                var userSearchService = scope.ServiceProvider.GetRequiredService<IUserSearchService>();
                await userSearchService.UpsertUserAsync(userEvent);

                await _channel.BasicAckAsync(ea.DeliveryTag, false);
            }
            catch (Exception ex)
            {
                if (ea.Redelivered == false)
                    await _channel.BasicNackAsync(ea.DeliveryTag, false, true);
                else
                    await _channel.BasicNackAsync(ea.DeliveryTag, false, false);

                _logger.LogError(ex, $"Failed to process message: {ea.Body}");
            }
        };

        await _channel.BasicConsumeAsync(queue: _queueName, autoAck: false, consumer: consumer);

        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_channel is not null)
        {
            await _channel.CloseAsync();
            await _channel.DisposeAsync();
        }
        if (_connection is not null)
        {
            await _connection.CloseAsync();
            await _connection.DisposeAsync();
        }
        await base.StopAsync(cancellationToken);
    }
}

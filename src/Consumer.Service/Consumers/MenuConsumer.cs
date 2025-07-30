using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using Newtonsoft.Json;
using Common.DTOs;
using MenuService.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ConsumerService.Consumers
{
    public class MenuConsumer : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<MenuConsumer> _logger;
        private readonly IConfiguration _configuration;

        public MenuConsumer(IServiceProvider serviceProvider, ILogger<MenuConsumer> logger, IConfiguration configuration)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _configuration = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var rabbitMQHost = _configuration["RabbitMQ:Host"] ?? "rabbitmq";
            var rabbitMQPort = int.Parse(_configuration["RabbitMQ:Port"] ?? "5672");
            var rabbitMQUsername = _configuration["RabbitMQ:Username"] ?? "guest";
            var rabbitMQPassword = _configuration["RabbitMQ:Password"] ?? "guest";

            var factory = new ConnectionFactory()
            {
                HostName = rabbitMQHost,
                Port = rabbitMQPort,
                UserName = rabbitMQUsername,
                Password = rabbitMQPassword
            };

            try
            {
                using var connection = await factory.CreateConnectionAsync();
                using var channel = await connection.CreateChannelAsync();

                const string queueName = "Menu.Service";

                await channel.QueueDeclareAsync(
                    queue: queueName,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                var consumer = new AsyncEventingBasicConsumer(channel);
                consumer.ReceivedAsync += async (sender, eventArgs) =>
                {
                    var message = eventArgs.Body.ToArray();
                    var json = Encoding.UTF8.GetString(message);

                    try
                    {
                        // Aqui você pode processar a mensagem conforme necessário
                        // Por exemplo, deserializar para um DTO específico do menu
                        _logger.LogInformation("Menu message received: {Message}", json);

                        // Processar a mensagem aqui se necessário
                        // using var scope = _serviceProvider.CreateScope();
                        // var dbContext = scope.ServiceProvider.GetRequiredService<MenuDbContext>();
                        // ... processar dados ...

                        await channel.BasicAckAsync(eventArgs.DeliveryTag, false);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error processing menu message: {Message}", json);
                        await channel.BasicNackAsync(eventArgs.DeliveryTag, false, true);
                    }
                };

                await channel.BasicConsumeAsync(queue: queueName, autoAck: false, consumer: consumer);

                _logger.LogInformation("MenuConsumer started and listening for messages");
                await Task.Delay(Timeout.Infinite, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in MenuConsumer");
                throw;
            }
        }
    }
}

using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using Newtonsoft.Json;
using KitchenService.Data;
using KitchenService.Model;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Common.DTOs;
using OrderService.Models;

namespace ConsumerService.Consumers
{
    public class KitchenConsumer : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<KitchenConsumer> _logger;
        private readonly IConfiguration _configuration;

        public KitchenConsumer(IServiceProvider serviceProvider, ILogger<KitchenConsumer> logger, IConfiguration configuration)
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

            var factory = new ConnectionFactory
            {
                HostName = rabbitMQHost,
                UserName = rabbitMQUsername,
                Password = rabbitMQPassword,
                Port = rabbitMQPort,
            };

            try
            {
                using var connection = await factory.CreateConnectionAsync();
                using var channel = await connection.CreateChannelAsync();

                await channel.ExchangeDeclareAsync(exchange: "order_exchange", type: ExchangeType.Fanout, durable: true);
                await channel.QueueDeclareAsync(queue: "Kitchen.Service", durable: true, exclusive: false, autoDelete: false);
                await channel.QueueBindAsync(queue: "Kitchen.Service", exchange: "order_exchange", routingKey: "");

                var consumer = new AsyncEventingBasicConsumer(channel);

                consumer.ReceivedAsync += async (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);

                    try
                    {
                        var kitchenOrderDto = JsonConvert.DeserializeObject<KitchenOrderDto>(message);

                        if (kitchenOrderDto != null)
                        {
							var orderMap = new KitchenOrder
							{
								CustomerId = kitchenOrderDto.CustomerId,
								CustomerName = kitchenOrderDto.CustomerName,
								TotalAmount = kitchenOrderDto.TotalAmount,
								DeliveryType = kitchenOrderDto.DeliveryType,
								Status = kitchenOrderDto.Status,
								CreatedAt = kitchenOrderDto.CreatedAt, 
								CancellationReason = kitchenOrderDto.CancellationReason,
								KitchenOrderItems = kitchenOrderDto.Items.Select(i => new KitchenOrderItem
								{
									MenuItemId = i.MenuItemId,
									MenuItemName = i.MenuItemName,
									Quantity = i.Quantity,
									UnitPrice = i.UnitPrice,
									TotalPrice = i.TotalPrice
								}).ToList()
							};


							using var scope = _serviceProvider.CreateScope();
                            var dbContext = scope.ServiceProvider.GetRequiredService<KitchenDbContext>();

                            dbContext.KitchenOrders.Add(orderMap);
                            await dbContext.SaveChangesAsync(stoppingToken);

                            _logger.LogInformation("Kitchen order {OrderId} processed successfully", orderMap.Id);
                            await channel.BasicAckAsync(ea.DeliveryTag, false, stoppingToken);
                        }
                        else
                        {
                            _logger.LogWarning("Failed to deserialize kitchen order message: {Message}", message);
                            await channel.BasicNackAsync(ea.DeliveryTag, false, false, stoppingToken);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error processing kitchen order message: {Message}", message);
                        await channel.BasicNackAsync(ea.DeliveryTag, false, true, stoppingToken);
                    }
                };

                await channel.BasicConsumeAsync(
                    queue: "Kitchen.Service",
                    autoAck: false,
                    consumer: consumer,
                    cancellationToken: stoppingToken);

                _logger.LogInformation("KitchenConsumer started and listening for messages");
                await Task.Delay(Timeout.Infinite, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in KitchenConsumer");
                throw;
            }
        }
    }
}

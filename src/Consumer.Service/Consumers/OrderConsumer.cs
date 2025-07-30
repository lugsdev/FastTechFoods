using OrderService.Data;
using OrderService.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using Newtonsoft.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Common.DTOs;
using KitchenService.Model;
using KitchenService.Data;

namespace ConsumerService.Consumers
{
    public class OrderConsumer : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<OrderConsumer> _logger;
        private readonly IConfiguration _configuration;

        public OrderConsumer(IServiceProvider serviceProvider, ILogger<OrderConsumer> logger, IConfiguration configuration)
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
                await channel.QueueDeclareAsync(queue: "Order.Service", durable: true, exclusive: false, autoDelete: false);
                await channel.QueueBindAsync(queue: "Order.Service", exchange: "order_exchange", routingKey: "");

                var consumer = new AsyncEventingBasicConsumer(channel);

                consumer.ReceivedAsync += async (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);

                    try
                    {
                        var orderDto = JsonConvert.DeserializeObject<OrderDto>(message);

                        if (orderDto != null)
                        {
                            var orderMap = new Order
                            {
                                CustomerId = orderDto.CustomerId,
                                CustomerName = orderDto.CustomerName,
                                TotalAmount = orderDto.TotalAmount,
                                DeliveryType = orderDto.DeliveryType,
                                Status = orderDto.Status,
                                CreatedAt = orderDto.CreatedAt,
                                CancellationReason = orderDto.CancellationReason,
                                Items = orderDto.Items.Select(i => new OrderItem
                                {
                                    MenuItemId = i.MenuItemId,
                                    MenuItemName = i.MenuItemName,
                                    Quantity = i.Quantity,
                                    UnitPrice = i.UnitPrice,
                                    TotalPrice = i.TotalPrice
                                }).ToList()
                            };

                            using var scope = _serviceProvider.CreateScope();
                            var dbContext = scope.ServiceProvider.GetRequiredService<OrderDbContext>();

                            dbContext.Orders.Add(orderMap);
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
                    queue: "Order.Service",
                    autoAck: false,
                    consumer: consumer,
                    cancellationToken: stoppingToken);

                _logger.LogInformation("OrderConsumer started and listening for messages");
                await Task.Delay(Timeout.Infinite, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in OrderConsumer");
                throw;
            }
        }
    }
}

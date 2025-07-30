using Common.DTOs;
using Microsoft.AspNetCore.Connections;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace MenuService.Messaging
{
	public class Publisher
	{
		public Publisher(IConfiguration configuration)
		{
		}

		public async Task Publish(MenuItemDto menuItemDto)
		{
			var factory = new ConnectionFactory()
			{
				HostName = "rabbitmq",
				Port = 5672,
				UserName = "guest",
				Password = "guest"
			};
			using var connection = await factory.CreateConnectionAsync();
			using var channel = await connection.CreateChannelAsync();

			await channel.QueueDeclareAsync(queue: "Menu.Service", durable: true, exclusive: false, autoDelete: false);
			await channel.ExchangeDeclareAsync(exchange: "order_exchange", type: ExchangeType.Fanout, durable: true);

			var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(menuItemDto));

			await channel.BasicPublishAsync(exchange: "order_exchange", routingKey: "", body: body);

		}
	}
}

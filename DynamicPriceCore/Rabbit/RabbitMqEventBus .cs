using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

public interface IEventBus : IAsyncDisposable
{
	Task PublishAsync<T>(T @event, string routingKey);
}

public class RabbitMqEventBus : IEventBus, IAsyncDisposable
{
	private readonly IConnection _connection;
	private readonly IChannel _channel;
	private const string ExchangeName = "company_monitoring_exchange";

	public RabbitMqEventBus(string connectionString)
	{
		try
		{
			var factory = new ConnectionFactory { Uri = new Uri(connectionString) };
			_connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();
			_channel = _connection.CreateChannelAsync().GetAwaiter().GetResult();

			_channel.ExchangeDeclareAsync(exchange: ExchangeName, type: ExchangeType.Direct, durable: true).Wait();
		}
		catch (Exception ex) 
		{

		}
	}

	public async Task PublishAsync<T>(T @event, string routingKey)
	{
		var json = JsonSerializer.Serialize(@event);
		var body = Encoding.UTF8.GetBytes(json);

		await _channel.BasicPublishAsync(
			exchange: ExchangeName,
			routingKey: routingKey,
			body: body
		);
	}

	public async ValueTask DisposeAsync()
	{
		await _channel.CloseAsync();
		await _connection.CloseAsync();
	}
}

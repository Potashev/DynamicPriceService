using DynamicPriceCore.Data;
using DynamicPriceCore.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;
using DynamicPrice.Core.Rabbit;

namespace DynamicPriceCore.Services;

public class ActiveCompaniesService : IActiveCompaniesService, IAsyncDisposable
{
	private readonly IServiceProvider _serviceProvider;
	private readonly IEventBus _eventBus;
	private readonly ConnectionFactory _factory;
	private IConnection? _connection;
	private IChannel? _channel;

	private readonly ConcurrentDictionary<int, bool> _activeCompanies = new();

	private const string ExchangeName = "company_monitoring_exchange";
	private const string QueueName = "company_monitoring_queue";

	public ActiveCompaniesService(IServiceProvider serviceProvider, IConfiguration config, IEventBus eventBus)
	{
		_serviceProvider = serviceProvider;
		_eventBus = eventBus;

		var rabbitMqConnStr = config.GetConnectionString("RabbitMQ") ?? "amqp://guest:guest@localhost:5672/";
		_factory = new ConnectionFactory
		{
			Uri = new Uri(rabbitMqConnStr)
		};

		_ = Task.Run(StartConsumerAsync);
	}

	public async Task StartConsumerAsync()
	{
		_connection = await _factory.CreateConnectionAsync();
		_channel = await _connection.CreateChannelAsync();

		await _channel.ExchangeDeclareAsync(exchange: ExchangeName, type: ExchangeType.Direct, durable: true);
		await _channel.QueueDeclareAsync(queue: QueueName, durable: true, exclusive: false, autoDelete: false);
		await _channel.QueueBindAsync(queue: QueueName, exchange: ExchangeName, routingKey: "company.start");
		await _channel.QueueBindAsync(queue: QueueName, exchange: ExchangeName, routingKey: "company.stop");

		var consumer = new AsyncEventingBasicConsumer(_channel);
		consumer.ReceivedAsync += async (sender, ea) =>
		{
			try
			{
				var json = Encoding.UTF8.GetString(ea.Body.ToArray());

				if (ea.RoutingKey == "company.start")
				{
					var evt = JsonSerializer.Deserialize<CompanyMonitoringStarted>(json);
					if (evt != null)
					{
						if (_activeCompanies.TryAdd(evt.CompanyId, true))
						{
							// 🔥 сразу публикуем в отдельную очередь, чтобы воркер начал мониторинг
							await _eventBus.PublishAsync(new { CompanyId = evt.CompanyId }, "company.monitoring");
						}
					}
				}
				else if (ea.RoutingKey == "company.stop")
				{
					var evt = JsonSerializer.Deserialize<CompanyMonitoringStopped>(json);
					if (evt != null)
					{
						_activeCompanies.TryRemove(evt.CompanyId, out _);
						// ❗️Можно отправить событие "остановки мониторинга"
						await _eventBus.PublishAsync(new { CompanyId = evt.CompanyId }, "company.monitoring.stop");
					}
				}

				await _channel!.BasicAckAsync(ea.DeliveryTag, multiple: false);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"[ActiveCompaniesService] Error handling message: {ex}");
			}
		};

		await _channel.BasicConsumeAsync(queue: QueueName, autoAck: false, consumer: consumer);
	}

	public IEnumerable<Company> GetActiveCompanies()
	{
		if (_activeCompanies.IsEmpty) return Enumerable.Empty<Company>();

		var activeCompanyIds = _activeCompanies.Keys.ToArray();

		using var scope = _serviceProvider.CreateScope();
		var context = scope.ServiceProvider.GetRequiredService<DynamicPriceCoreContext>();

		return context.Companies
			.Where(c => activeCompanyIds.Contains(c.CompanyId))
			.ToList();
	}

	public bool IsActive(int companyId) => _activeCompanies.ContainsKey(companyId);

	public async ValueTask DisposeAsync()
	{
		if (_channel != null) await _channel.CloseAsync();
		if (_connection != null) await _connection.CloseAsync();
	}
}

public interface IActiveCompaniesService
{
	IEnumerable<Company> GetActiveCompanies();
	bool IsActive(int companyId);
}

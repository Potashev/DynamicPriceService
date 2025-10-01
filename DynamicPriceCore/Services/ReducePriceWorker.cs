using DynamicPrice.Core.Benchmark;
using DynamicPrice.Core.Services;
using DynamicPriceCore.Data;
using DynamicPriceCore.Models;
using Microsoft.EntityFrameworkCore;
using Prometheus;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;

public class ReducePriceWorker : BackgroundService
{
	private readonly IServiceProvider _serviceProvider;
	private readonly ConnectionFactory _factory;
	private IConnection? _connection;
	private IChannel? _channel;

	private static readonly Histogram MonitorDuration = Metrics
	.CreateHistogram("dp_company_monitor_duration_seconds",
		"Время выполнения мониторинга компании",
		new HistogramConfiguration
		{
			LabelNames = new[] { "companyId" }
		});
	private static readonly Histogram MonitorWaitDuration = Metrics
	.CreateHistogram("dp_company_monitor_wait_seconds",
		"Время ожидания до следующего мониторинга компании",
		new HistogramConfiguration
		{
			LabelNames = new[] { "companyId" }
		});
	private readonly ConcurrentDictionary<int, DateTime> _lastMonitorEnd = new();

	private readonly ConcurrentDictionary<int, CancellationTokenSource> _companyMonitors = new();

	private const string ExchangeName = "company_monitoring_exchange";
	private const string QueueName = "company_monitoring_worker";

	public ReducePriceWorker(IServiceProvider serviceProvider, IConfiguration config)
	{
		_serviceProvider = serviceProvider;
		_factory = new ConnectionFactory
		{
			Uri = new Uri(config.GetConnectionString("RabbitMQ") ?? "amqp://guest:guest@localhost:5672/")
		};
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		_connection = await _factory.CreateConnectionAsync();
		_channel = await _connection.CreateChannelAsync();

		await _channel.ExchangeDeclareAsync(exchange: ExchangeName, type: ExchangeType.Direct, durable: true);
		await _channel.QueueDeclareAsync(queue: QueueName, durable: true, exclusive: false, autoDelete: false);

		await _channel.QueueBindAsync(queue: QueueName, exchange: ExchangeName, routingKey: "company.monitoring");
		await _channel.QueueBindAsync(queue: QueueName, exchange: ExchangeName, routingKey: "company.monitoring.stop");

		//todo: check
		await _channel.QueueDeclareAsync(
			queue: "price.reduce",
			durable: true,
			exclusive: false,
			autoDelete: false,
			arguments: null);

		var consumer = new AsyncEventingBasicConsumer(_channel);
		consumer.ReceivedAsync += async (_, ea) =>
		{
			var json = Encoding.UTF8.GetString(ea.Body.ToArray());
			var payload = JsonSerializer.Deserialize<CompanyPayload>(json);

			if (payload != null)
			{
				if (ea.RoutingKey == "company.monitoring")
					StartMonitoringForCompany(payload.CompanyId);
				else if (ea.RoutingKey == "company.monitoring.stop")
					StopMonitoringForCompany(payload.CompanyId);
			}

			await _channel!.BasicAckAsync(ea.DeliveryTag, false);
		};

		await _channel.BasicConsumeAsync(queue: QueueName, autoAck: false, consumer: consumer);
	}

	private void StartMonitoringForCompany(int companyId)
	{
		if (_companyMonitors.ContainsKey(companyId))
		{
			Console.WriteLine($"[ReducePriceWorker] Мониторинг уже запущен для {companyId}");
			return;
		}

		var cts = new CancellationTokenSource();
		if (_companyMonitors.TryAdd(companyId, cts))
		{
			Console.WriteLine($"[ReducePriceWorker] Старт мониторинга для компании {companyId}");
			_ = Task.Run(() => MonitorLoop(companyId, cts.Token));
		}
	}

	private void StopMonitoringForCompany(int companyId)
	{
		if (_companyMonitors.TryRemove(companyId, out var cts))
		{
			Console.WriteLine($"[ReducePriceWorker] Остановка мониторинга для компании {companyId}");
			cts.Cancel();
		}
	}

	private async Task MonitorLoop(int companyId, CancellationToken token)
	{
		try
		{
			while (!token.IsCancellationRequested)
			{
				// --- Мониторинг ---
				using (MonitorDuration.WithLabels(companyId.ToString()).NewTimer())
				{
					using var scope = _serviceProvider.CreateScope();
					var context = scope.ServiceProvider.GetRequiredService<DynamicPriceCoreContext>();

					var monitor = new CompanyMonitor(context);
					var productsToReduce = await monitor.FindProductsToReduceAsync(companyId, token);

					if (productsToReduce != null)
					{
						foreach (var productId in productsToReduce)
						{
							var message = new PriceReduceMessage(productId, companyId);
							var json = JsonSerializer.Serialize(message);
							var body = Encoding.UTF8.GetBytes(json);

							await _channel!.BasicPublishAsync(
								exchange: "",
								routingKey: "price.reduce",
								body: body);
						}
					}
				}

				// --- Зафиксировать окончание мониторинга ---
				_lastMonitorEnd[companyId] = DateTime.UtcNow;

				// --- Ждём до следующего цикла ---
				await Task.Delay(TimeSpan.FromSeconds(1), token);

				// --- Измеряем ожидание ---
				if (_lastMonitorEnd.TryGetValue(companyId, out var lastEnd))
				{
					var waitSeconds = (DateTime.UtcNow - lastEnd).TotalSeconds;
					MonitorWaitDuration.WithLabels(companyId.ToString()).Observe(waitSeconds);
				}
			}
		}
		catch (OperationCanceledException)
		{
			// ignore
		}
	}

	public record PriceReduceMessage(int ProductId, int CompanyId);
	public record CompanyPayload(int CompanyId);
}

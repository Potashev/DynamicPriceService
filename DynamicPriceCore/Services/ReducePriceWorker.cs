using DynamicPriceCore.Data;
using DynamicPriceCore.Models;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

public class ReducePriceWorker : BackgroundService
{
	private readonly IServiceProvider _serviceProvider;
	private readonly ConnectionFactory _factory;
	private IConnection? _connection;
	private IChannel? _channel;

	private const string ExchangeName = "company_monitoring_exchange";
	private const string QueueName = "company_monitoring_worker";

	public ReducePriceWorker(IServiceProvider serviceProvider, IConfiguration config)
	{
		_serviceProvider = serviceProvider;

		var rabbitMqConnStr = config.GetConnectionString("RabbitMQ") ?? "amqp://guest:guest@localhost:5672/";
		_factory = new ConnectionFactory
		{
			Uri = new Uri(rabbitMqConnStr)
		};
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		_connection = await _factory.CreateConnectionAsync();
		_channel = await _connection.CreateChannelAsync();

		await _channel.ExchangeDeclareAsync(exchange: ExchangeName, type: ExchangeType.Direct, durable: true);
		await _channel.QueueDeclareAsync(queue: QueueName, durable: true, exclusive: false, autoDelete: false);
		await _channel.QueueBindAsync(queue: QueueName, exchange: ExchangeName, routingKey: "company.monitoring");

		var consumer = new AsyncEventingBasicConsumer(_channel);
		consumer.ReceivedAsync += async (_, ea) =>
		{
			var json = Encoding.UTF8.GetString(ea.Body.ToArray());
			var payload = JsonSerializer.Deserialize<CompanyPayload>(json);

			if (payload != null)
				await StartMonitoringForCompanyAsync(payload.CompanyId, stoppingToken);

			await _channel.BasicAckAsync(ea.DeliveryTag, false);
		};

		await _channel.BasicConsumeAsync(queue: QueueName, autoAck: false, consumer: consumer);
	}

	private async Task StartMonitoringForCompanyAsync(int companyId, CancellationToken token)
	{
		Console.WriteLine($"[ReducePriceWorker] Старт мониторинга для компании {companyId}");

		while (!token.IsCancellationRequested)
		{
			using var scope = _serviceProvider.CreateScope();
			var context = scope.ServiceProvider.GetRequiredService<DynamicPriceCoreContext>();

			var priceRule = context.PriceRules.Where(pr => pr.Company.CompanyId == companyId).FirstOrDefault();

			var productsToReduceQuery = from p in context.Products
										where
									   p.Company.CompanyId == companyId &&
									   EF.Functions.DateDiffSecond(p.LastSellTime, DateTime.UtcNow) > priceRule.NoSellTime.Value.TotalSeconds
										select p;

			var productsToReduce = productsToReduceQuery.ToList();


			// 🔍 Получаем продукты для этой компании и проверяем на "простой"
			//var products = context.Products
			//	.Where(p => p.CompanyId == companyId /* и условия простоя */)
			//	.ToList();

			foreach (var product in productsToReduce)
			{
				Console.WriteLine($"[ReducePriceWorker] Нашли продукт для снижения цены: {product.ProductId}");
				// тут можно публиковать событие в очередь "price.reduce"
			}

			await Task.Delay(TimeSpan.FromSeconds(10), token); // например, каждые 10 сек
		}
	}

	public record CompanyPayload(int CompanyId);
}

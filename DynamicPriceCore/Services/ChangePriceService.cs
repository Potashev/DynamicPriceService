using DynamicPriceCore.Data;
using DynamicPriceCore.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Prometheus;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace DynamicPrice.Core.Services;

public class ChangePriceService : BackgroundService
{
	private readonly IServiceProvider _serviceProvider;
	private readonly ConnectionFactory _factory;
	private IConnection? _connection;
	private IChannel? _channel;

	private readonly IHubContext<PriceHub> _priceHubContext;

	private static readonly Histogram ChangePriceDuration = Metrics
	.CreateHistogram("dp_changeprice_duration_seconds",
		"Время обработки события изменения цены",
		new HistogramConfiguration
		{
			LabelNames = new[] { "companyId" }
		});

	public ChangePriceService(IServiceProvider serviceProvider, IConfiguration config, IHubContext<PriceHub> priceHubContext)	//todo: remove PriceHub?
	{
		_serviceProvider = serviceProvider;
		_factory = new ConnectionFactory
		{
			Uri = new Uri(config.GetConnectionString("RabbitMQ") ?? "amqp://guest:guest@localhost:5672/")
		};

		_priceHubContext = priceHubContext;
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		_connection = await _factory.CreateConnectionAsync();
		_channel = await _connection.CreateChannelAsync();

		await _channel.QueueDeclareAsync("price.reduce", durable: true, exclusive: false, autoDelete: false);

		var consumer = new AsyncEventingBasicConsumer(_channel);
		consumer.ReceivedAsync += async (_, ea) =>
		{
			var json = Encoding.UTF8.GetString(ea.Body.ToArray());
			var msg = JsonSerializer.Deserialize<PriceReduceMessage>(json);

			if (msg != null)
			{
				using (ChangePriceDuration.WithLabels(msg.CompanyId.ToString()).NewTimer())
				{
					await HandlePriceReduction(msg);
				}
			}

			await _channel.BasicAckAsync(ea.DeliveryTag, false);
		};

		await _channel.BasicConsumeAsync("price.reduce", autoAck: false, consumer: consumer);
	}

	private async Task HandlePriceReduction(PriceReduceMessage msg)
	{
		using var scope = _serviceProvider.CreateScope();
		var context = scope.ServiceProvider.GetRequiredService<DynamicPriceCoreContext>();

		var product = await context.Products
			.FirstOrDefaultAsync(p => p.ProductId == msg.ProductId);

		if (product == null) return;

		var priceRule = await context.PriceRules
			.FirstOrDefaultAsync(r => r.Company.CompanyId == product.CompanyId);

		if (priceRule == null) return;

		product.Price = Math.Max(
			ReducePrice(product.Price, priceRule.Reduction, true),
			product.MinimumPrice);



		await context.PriceDynamics.AddAsync(new PriceDynamic
		{
			Product = product,
			Price = product.Price,
			Date = DateTime.UtcNow
		});

		await context.SaveChangesAsync();

		await _priceHubContext.Clients.All.SendAsync("ReceivePriceUpdate", product.ProductId, product.Price);
	}

	private decimal ReducePrice(decimal price, double pricingRuleReduction, bool testDrawing = false)
	{
		var reduction = (decimal)pricingRuleReduction * 0.01m * price; //todo: think about rounding
		price -= reduction;

		//todo: temp field for checking drawing - remove after test
		if (testDrawing)
		{
			var maxrand = (int)Math.Round(reduction * 2);
			var rnd = new Random();
			price += rnd.Next(maxrand);
		}

		return price;
	}

	//todo: think about remove companyId from message
	public record PriceReduceMessage(int ProductId, int CompanyId);
}


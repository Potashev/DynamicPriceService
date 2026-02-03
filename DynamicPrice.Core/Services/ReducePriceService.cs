using DynamicPrice.Core.Data;
using DynamicPrice.Core.Models;
using DynamicPrice.Core.SignalR;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Prometheus;

namespace DynamicPrice.Core.Services;

/// <summary>
/// Сервис, обрабатывающий события снижения цены продуктов.
/// Получает событие <see cref="PriceReduceEvent"/> и увеличивает цену соответствующего продукта
/// согласно правилу снижения цены компании <see cref="PriceRule.Reduction"/>.
/// После изменения цены уведомляет всех подписавшихся клиентов через SignalR-хаб <see cref="PriceHub"/>.
/// </summary>
public class ReducePriceService : IConsumer<PriceReduceEvent>
{
	private readonly IServiceProvider _serviceProvider;
	private readonly IHubContext<PriceHub> _priceHubContext;
	private readonly ILogger<ReducePriceService> _logger;

	private static readonly Histogram ChangePriceDuration = Metrics
	.CreateHistogram("dp_changeprice_duration_seconds",
		"Время обработки события изменения цены",
		new HistogramConfiguration
		{
			LabelNames = new[] { "companyId" }
		});

	public ReducePriceService(
		IServiceProvider serviceProvider,
		IConfiguration config,
		IHubContext<PriceHub> priceHubContext,
		ILogger<ReducePriceService> logger)
	{
		_serviceProvider = serviceProvider;
		_priceHubContext = priceHubContext;
		_logger = logger;
	}

	public async Task Consume(ConsumeContext<PriceReduceEvent> context)
	{
		var msg = context.Message;
		if (msg != null)
		{
			//using (ChangePriceDuration.WithLabels(message.CompanyId.ToString()).NewTimer())
			//{
			await ReducePrice(msg.ProductId);
			//}
		}
	}

	private async Task ReducePrice(int productId)
	{
		//todo: use logger
		using var scope = _serviceProvider.CreateScope();
		var context = scope.ServiceProvider.GetRequiredService<DynamicPriceCoreContext>();

		var product = await context.Products
			.FirstOrDefaultAsync(p => p.ProductId == productId);

		if (product == null) return;

		var priceRule = await context.PriceRules
			.FirstOrDefaultAsync(r => r.Company.CompanyId == product.CompanyId);

		if (priceRule == null) return;

		product.Price = Math.Max(
					//ReducePrice(product.Price, priceRule.Reduction, true),
					ReducePrice(product.Price, priceRule.Reduction),
			product.MinimumPrice);

		await context.PriceDynamics.AddAsync(new PriceDynamic
		{
			ProductId = product.ProductId,
			Price = product.Price,
			Date = DateTime.UtcNow
		});

		await context.SaveChangesAsync();

		await NoticeOfReduce(product);
	}

	private decimal ReducePrice(
		decimal price,
		double pricingRuleReduction,
		bool testDrawing = false)
	{
		var reduction = (decimal)pricingRuleReduction * 0.01m * price;
		price -= reduction;

		// Test seed data for checking drawing
		if (testDrawing)
		{
			var maxrand = (int)Math.Round(reduction * 2);
			var rnd = new Random();
			price += rnd.Next(maxrand);
		}

		return price;
	}

	private async Task NoticeOfReduce(Product product)
	{
		try
		{
			await _priceHubContext.SendPriceUpdateToProductGroup(product.ProductId, product.Price);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Failed to notify hub about product {ProductId}", product.ProductId);
		}
	}
}


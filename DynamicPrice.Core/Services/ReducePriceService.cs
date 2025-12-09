using DynamicPrice.Core.Data;
using DynamicPrice.Core.Models;
using DynamicPrice.Core.SignalR;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Prometheus;

namespace DynamicPrice.Core.Services;

/// <summary>
/// Сервис, обрабатывающий события снижения цены (консьюмер для MassTransit).
/// Вычисляет новую цену для конкретного товара в соответствии с правилом компании,
/// сохраняет запись в истории цен и уведомляет клиентов через SignalR.
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

	/// <summary>
	/// Создаёт экземпляр сервиса снижения цены.
	/// </summary>
	public ReducePriceService(IServiceProvider serviceProvider, IConfiguration config, IHubContext<PriceHub> priceHubContext, ILogger<ReducePriceService> logger)
	{
		_serviceProvider = serviceProvider;
		_priceHubContext = priceHubContext;
		_logger = logger;
	}

	/// <summary>
	/// Обработчик сообщения <see cref="PriceReduceEvent"/>. Делегирует основную работу в <see cref="ReducePrice"/>
	/// </summary>
	/// <param name="context">Контекст сообщения MassTransit.</param>
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

	/// <summary>
	/// Вычисляет и применяет уменьшенную цену для товара, записывает историю и уведомляет клиентов.
	/// </summary>
	/// <param name="productId">Идентификатор товара для обработки.</param>
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
					//todo: check increasePriceService.NoticeOfIncrease (testdrawing = false)
					ReducePrice(product.Price, priceRule.Reduction, true),
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

	/// <summary>
	/// Вычисление новой уменьшенной цены по правилу в процентах.
	/// При тестировании может добавляться случайная дельта (testDrawing) — пометка разработчика.
	/// </summary>
	/// <param name="price">Текущая цена.</param>
	/// <param name="pricingRuleReduction">Процент снижения (например, 10 — означает 10%).</param>
	/// <param name="testDrawing">Флаг тестового добавления случайной дельты.</param>
	/// <returns>Новая цена после уменьшения.</returns>
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

	/// <summary>
	/// Отправляет уведомление об изменении цены через SignalR. Ошибки логируются.
	/// </summary>
	/// <param name="product">Сущность товара с обновлённой ценой.</param>
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


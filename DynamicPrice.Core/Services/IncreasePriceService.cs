using DynamicPrice.Core.Data;
using DynamicPrice.Core.Models;
using DynamicPrice.Core.SignalR;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace DynamicPrice.Core.Services;

//todo: think about base ChangePriceService and move common for Increase and Reduce
/// <summary>
/// Сервис, обрабатывающий события повышения цены товаров (консьюмер для MassTransit).
/// Получает событие с позициями заказа и увеличивает цену соответствующих товаров
/// согласно правилу повышения цены компании, сохраняет историю и уведомляет клиентов через SignalR.
/// </summary>
public class IncreasePriceService : IConsumer<PriceIncreaseEvent>
{
	private readonly IServiceProvider _serviceProvider;
	private readonly IHubContext<PriceHub> _priceHubContext;
	private readonly ILogger<IncreasePriceService> _logger;

	/// <summary>
	/// Создаёт экземпляр сервиса изменения цен.
	/// </summary>
	public IncreasePriceService(IServiceProvider serviceProvider, IHubContext<PriceHub> priceHubContext, ILogger<IncreasePriceService> logger)
	{
		_serviceProvider = serviceProvider;
		_priceHubContext = priceHubContext;
		_logger = logger;
	}

	/// <summary>
	/// Обработчик входящего сообщения <see cref="PriceIncreaseEvent"/> от шины сообщений.
	/// Гарантирует логирование ошибок и делегирует обработку в <see cref="IncreasePrices"/>.
	/// </summary>
	/// <param name="context">Контекст сообщения MassTransit с данными о позициях заказа.</param>
	public async Task Consume(ConsumeContext<PriceIncreaseEvent> context)
	{
		var msg = context.Message;

		if (msg != null)
		{
			try
			{
				await IncreasePrices(msg.OrderItems);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error processing PriceIncreaseEvent");
				throw;
			}
		}
	}

	/// <summary>
	/// Основная логика повышения цен для переданных позиций заказа.
	/// Выполняется в скоупе DI, сохраняет изменения в БД и отправляет уведомления.
	/// </summary>
	/// <param name="OrderItems">Коллекция позиций заказа с продуктами и количеством.</param>
	private async Task IncreasePrices(IEnumerable<OrderItem> OrderItems)
	{
		using var scope = _serviceProvider.CreateScope();
		var context = scope.ServiceProvider.GetRequiredService<DynamicPriceCoreContext>();

		var companyId = OrderItems.FirstOrDefault()?.Product.CompanyId;
		var priceRule = await context.PriceRules
			.FirstOrDefaultAsync(p => p.Company.CompanyId == companyId);

		if (priceRule == null)
		{
			_logger.LogWarning("PriceRule not found for company {CompanyId}", companyId);
			return;
		}

		IncreasePrice(OrderItems, priceRule);

		await context.SaveChangesAsync();

		await NoticeOfIncrease(OrderItems);
	}

	/// <summary>
	/// Изменяет текущую цену продуктов в коллекции на основании правила повышения.
	/// Влияет только на поле <see cref="Product.Price"/> каждого продукта.
	/// </summary>
	/// <param name="OrderItems">Позиции заказа.</param>
	/// <param name="priceRule">Правило ценообразования компании.</param>
	private void IncreasePrice(IEnumerable<OrderItem> OrderItems, PriceRule priceRule)
	{
		foreach (var orderProduct in OrderItems)
		{
			var product = orderProduct.Product;
			var increase = product.Price * (decimal)priceRule.Increase * 0.01m * orderProduct.Quantity;
			product.Price += increase;
		}
	}

	/// <summary>
	/// Отправляет уведомления в SignalR-группы для обновлённых продуктов.
	/// Любые ошибки логируются, но не прерывают основной поток выполнения.
	/// </summary>
	/// <param name="OrderItems">Позиции заказа для уведомления.</param>
	private async Task NoticeOfIncrease(IEnumerable<OrderItem> OrderItems)
	{
		//todo: check
		foreach (var orderProduct in OrderItems)
		{
			var product = orderProduct.Product;
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
}

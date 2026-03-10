using DynamicPrice.Core.Data;
using DynamicPrice.Core.Models;
using DynamicPrice.Core.SignalR;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace DynamicPrice.Core.Services;

/// <summary>
/// Сервис, обрабатывающий события повышения цены продуктов.
/// Получает событие <see cref="PriceIncreaseEvent"/> с позициями заказа и увеличивает цену соответствующих продуктов
/// согласно правилу повышения цены компании <see cref="PriceRule.Increase"/>.
/// После изменения цены уведомляет всех подписавшихся клиентов через SignalR-хаб <see cref="PriceHub"/>.
/// </summary>
public class IncreasePriceService : PriceServiceBase<PriceIncreaseEvent>
{
	//private readonly IServiceProvider _serviceProvider;
	//private readonly IHubContext<PriceHub> _priceHubContext;
	//private readonly ILogger<IncreasePriceService> _logger;

	public IncreasePriceService(
		IServiceProvider serviceProvider,
		IHubContext<PriceHub> priceHubContext,
		ILogger<IncreasePriceService> logger)
		: base(serviceProvider, priceHubContext, logger)
	{
	}

	protected override async Task ProcessMessage(PriceIncreaseEvent message)
	{
		await UpdatePrice(message.ProductId, (product, rule) =>
		{
			var increase = product.Price *
						(decimal)rule.Increase *
						0.01m *
						message.Quantity;

			return product.Price + increase;
		});
	}

	//public async Task Consume(ConsumeContext<PriceIncreaseEvent> context)
	//{
	//	var msg = context.Message;

	//	if (msg != null)
	//	{
	//		try
	//		{
	//			await IncreasePrice(msg.ProductId, msg.Quantity);
	//		}
	//		catch (Exception ex)
	//		{
	//			_logger.LogError(ex, "Error processing PriceIncreaseEvent");
	//			throw;
	//		}
	//	}
	//}

	//private async Task IncreasePrice(int productId, int quantity)
	//{
	//	using var scope = _serviceProvider.CreateScope();
	//	var context = scope.ServiceProvider.GetRequiredService<DynamicPriceCoreContext>();

	//	var product = await context.Products
	//		.FirstOrDefaultAsync(p => p.ProductId == productId);

	//	if (product == null) return;

	//	var priceRule = await context.PriceRules
	//		.FirstOrDefaultAsync(r => r.Company.CompanyId == product.CompanyId);

	//	if (priceRule == null) return;

	//	var priceIncrease = product.Price * (decimal)priceRule.Increase * 0.01m * quantity;	//todo: compare with reducing
	//	product.Price += priceIncrease;

	//	await context.SaveChangesAsync();

	//	await _priceHubContext.SendPriceUpdateToProductGroup(product.ProductId, product.Price);
	//}

	//private decimal
}

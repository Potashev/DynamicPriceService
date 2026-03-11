using DynamicPrice.Core.Data;
using DynamicPrice.Core.Models;
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
public class ReducePriceService : PriceServiceBase<PriceReduceEvent>
{
	public ReducePriceService(
		DynamicPriceCoreContext context,
		IConfiguration config,
		IHubContext<PriceHub> priceHubContext,
		ILogger<ReducePriceService> logger)
		: base(context, priceHubContext, logger)
	{
	}

	protected override async Task ProcessMessage(PriceReduceEvent message)
	{
		await UpdatePrice(message.ProductId, (product, rule) =>
		{
			var reduction = product.Price * 
						(decimal)rule.Reduction * 
						0.01m;

			var newPrice = product.Price - reduction;

			return Math.Max(newPrice, product.MinimumPrice);
		});
	}
}


using DynamicPrice.Core.Data;
using DynamicPrice.Core.Models;
using Microsoft.AspNetCore.SignalR;

namespace DynamicPrice.Core.Services;

/// <summary>
/// Сервис, обрабатывающий события повышения цены продуктов.
/// Получает событие <see cref="PriceIncreaseEvent"/> с позициями заказа и увеличивает цену соответствующих продуктов
/// согласно правилу повышения цены компании <see cref="PriceRule.Increase"/>.
/// После изменения цены уведомляет всех подписавшихся клиентов через SignalR-хаб <see cref="PriceHub"/>.
/// </summary>
public class IncreasePriceService : PriceServiceBase<PriceIncreaseEvent>
{
	public IncreasePriceService(
		DynamicPriceCoreContext context,
		IHubContext<PriceHub> priceHubContext,
		ILogger<IncreasePriceService> logger)
		: base(context, priceHubContext, logger) { }

	protected override async Task ProcessMessage(PriceIncreaseEvent message)
		=> await UpdatePrice(message.ProductId, (product, rule) =>
		{
			var increase = product.Price *
						(decimal)rule.Increase *
						0.01m *
						message.Quantity;

			return product.Price + increase;
		});
}

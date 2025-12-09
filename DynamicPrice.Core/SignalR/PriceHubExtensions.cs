using Microsoft.AspNetCore.SignalR;

namespace DynamicPrice.Core.SignalR;

/// <summary>
/// Вспомогательные расширения для работы с <see cref="PriceHub"/> через <see cref="IHubContext{PriceHub}"/>.
/// </summary>
public static class PriceHubExtensions
{
	/// <summary>
	/// Отправляет уведомление в группу SignalR, соответствующую продукту.
	/// Клиентам пересылается событие с именем "ReceivePriceUpdate" и аргументами (productId, price).
	/// </summary>
	/// <param name="hubContext">Контекст хаба SignalR.</param>
	/// <param name="productId">Идентификатор продукта.</param>
	/// <param name="price">Новая цена продукта.</param>
	/// <returns>Задача, представляющая асинхронную операцию отправки сообщения.</returns>
	public static Task SendPriceUpdateToProductGroup(this IHubContext<PriceHub> hubContext, int productId, decimal price)
		=> hubContext.Clients.Group(PriceHub.GetProductGroup(productId))
			.SendAsync("ReceivePriceUpdate", productId, price);
}

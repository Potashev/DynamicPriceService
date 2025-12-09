using Microsoft.AspNetCore.SignalR;

/// <summary>
/// SignalR-хаб для рассылки обновлений цен по продуктам.
/// Клиенты подписываются на группы по продукту и получают сообщения об изменении цены.
/// </summary>
public class PriceHub : Hub
{
	/// <summary>
	/// Возвращает имя группы SignalR для конкретного продукта.
	/// Используется для подписок и отправки сообщений в группу.
	/// </summary>
	/// <param name="productId">Идентификатор продукта.</param>
	/// <returns>Имя группы в формате "product:{productId}".</returns>
	public static string GetProductGroup(int productId) => $"product:{productId}";

	/// <summary>
	/// Подписать текущее соединение на обновления указанного продукта.
	/// </summary>
	/// <param name="productId">Идентификатор продукта.</param>
	public Task SubscribeToProduct(int productId)
		=> Groups.AddToGroupAsync(Context.ConnectionId, GetProductGroup(productId));

	/// <summary>
	/// Отписать текущее соединение от обновлений указанного продукта.
	/// </summary>
	/// <param name="productId">Идентификатор продукта.</param>
	public Task UnsubscribeFromProduct(int productId)
		=> Groups.RemoveFromGroupAsync(Context.ConnectionId, GetProductGroup(productId));
}
using Microsoft.AspNetCore.SignalR;

/// <summary>
/// SignalR-хаб для рассылки изменения цен по продуктам.
/// Клиенты подписываются на группы по продукту и получают сообщения об изменении цены.
/// Клиентом может быть как менеджер компании при учете продуктов, так и кастомер, отслеживающий цены на интересующие продукты.
/// </summary>
public class PriceHub : Hub
{
	public static string GetProductGroup(int productId) => $"product:{productId}";

	public Task SubscribeToProduct(int productId)
		=> Groups.AddToGroupAsync(Context.ConnectionId, GetProductGroup(productId));

	public Task UnsubscribeFromProduct(int productId)
		=> Groups.RemoveFromGroupAsync(Context.ConnectionId, GetProductGroup(productId));
}
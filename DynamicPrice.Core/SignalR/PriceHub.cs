using Microsoft.AspNetCore.SignalR;

public class PriceHub : Hub
{
	public static string GetProductGroup(int productId) => $"product:{productId}";

	// Клиенты вызывают этот метод, чтобы присоединиться к product:{id}
	public Task SubscribeToProduct(int productId)
		=> Groups.AddToGroupAsync(Context.ConnectionId, GetProductGroup(productId));

	// Клиенты вызывают этот метод, чтобы выйти из группы product:{id}
	public Task UnsubscribeFromProduct(int productId)
		=> Groups.RemoveFromGroupAsync(Context.ConnectionId, GetProductGroup(productId));

	// Серверный метод для отправки обновления цены целевой группе продукта
	public async Task SendPriceUpdate(int productId, decimal newPrice)
	{
		await Clients.Group(GetProductGroup(productId)).SendAsync("ReceivePriceUpdate", productId, newPrice);
	}
}
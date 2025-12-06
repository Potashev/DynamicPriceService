using Microsoft.AspNetCore.SignalR;

public class PriceHub : Hub
{
	public static string GetProductGroup(int productId) => $"product:{productId}";

	public Task SubscribeToProduct(int productId)
		=> Groups.AddToGroupAsync(Context.ConnectionId, GetProductGroup(productId));

	public Task UnsubscribeFromProduct(int productId)
		=> Groups.RemoveFromGroupAsync(Context.ConnectionId, GetProductGroup(productId));
}
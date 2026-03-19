using Microsoft.AspNetCore.SignalR;

namespace DynamicPrice.Core.SignalR;

public static class PriceHubExtensions
{
	public static Task SendPriceUpdateToProductGroup(
		this IHubContext<PriceHub> hubContext,
		int productId,
		decimal updatedPrice)
		=> hubContext.Clients.Group(PriceHub.GetProductGroup(productId))
			.SendAsync("ReceivePriceUpdate", productId, updatedPrice, new DateTime(2026,3,18,10,10,10));	// очень странно но даже так отображаем локальное время - как будто шлем не от сюда 
	//.SendAsync("ReceivePriceUpdate", productId, updatedPrice, DateTime.UtcNow);
	//.SendAsync("ReceivePriceUpdate", productId, updatedPrice);
}

using Microsoft.AspNetCore.SignalR;

namespace DynamicPrice.Core.SignalR;

public static class PriceHubExtensions
{
	public static Task SendPriceUpdateToProductGroup(
		this IHubContext<PriceHub> hubContext,
		int productId,
		decimal updatedPrice)
		=> hubContext.Clients.Group(PriceHub.GetProductGroup(productId))
			.SendAsync("ReceivePriceUpdate", productId, updatedPrice);
}

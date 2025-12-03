using Microsoft.AspNetCore.SignalR;

namespace DynamicPrice.Core.SignalR;

public static class PriceHubExtensions
{
	public static Task SendPriceUpdateToCompanyManagers(this IHubContext<PriceHub> hubContext, int companyId, int productId, decimal price)
		=> hubContext.Clients.Group(PriceHub.GetManagersGroup(companyId))
			.SendAsync("ReceivePriceUpdate", productId, price);

	public static Task SendPriceUpdateToCompanyViewers(this IHubContext<PriceHub> hubContext, int companyId, int productId, decimal price)
		=> hubContext.Clients.Group(PriceHub.GetCompanyGroup(companyId))
			.SendAsync("ReceivePriceUpdate", productId, price);

	public static Task SendPriceUpdateToProductGroup(this IHubContext<PriceHub> hubContext, int productId, decimal price)
		=> hubContext.Clients.Group(PriceHub.GetProductGroup(productId))
			.SendAsync("ReceivePriceUpdate", productId, price);

	// Отправить конкретному пользователю по userId (SignalR использует UserIdentifier, обычно ClaimTypes.NameIdentifier)
	public static Task SendPriceUpdateToUser(this IHubContext<PriceHub> hubContext, string userId, int productId, decimal price)
		=> hubContext.Clients.User(userId).SendAsync("ReceivePriceUpdate", productId, price);
}

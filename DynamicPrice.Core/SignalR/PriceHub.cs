using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

public class PriceHub : Hub
{
	// Этот метод может вызываться клиентом чтобы подписаться на группу компании (просмотр товаров)
	public Task SubscribeToCompany(int companyId)
		=> Groups.AddToGroupAsync(Context.ConnectionId, GetCompanyGroup(companyId));

	public Task UnsubscribeFromCompany(int companyId)
		=> Groups.RemoveFromGroupAsync(Context.ConnectionId, GetCompanyGroup(companyId));

	// Подписка для менеджеров (обычно UI менеджера вызывает этот метод или сервер делает это автоматически в OnConnected)
	public Task SubscribeManagers(int companyId)
		=> Groups.AddToGroupAsync(Context.ConnectionId, GetManagersGroup(companyId));

	public Task UnsubscribeManagers(int companyId)
		=> Groups.RemoveFromGroupAsync(Context.ConnectionId, GetManagersGroup(companyId));

	// (Опционально) подписка на конкретный продукт
	public Task SubscribeToProduct(int productId)
		=> Groups.AddToGroupAsync(Context.ConnectionId, GetProductGroup(productId));

	public Task UnsubscribeFromProduct(int productId)
		=> Groups.RemoveFromGroupAsync(Context.ConnectionId, GetProductGroup(productId));

	public override async Task OnConnectedAsync()
	{
		// если клиент аутентифицирован и это менеджер, автоматически добавляем его в менеджерскую группу своей компании
		var user = Context.User;
		if (user?.Identity?.IsAuthenticated == true)
		{
			var role = user.FindFirst(ClaimTypes.Role)?.Value;
			if (role == "Manager")
			{
				// ожидаем что claim CompanyId присутствует (или другой claim)
				var companyIdClaim = user.FindFirst("companyId") ?? user.FindFirst("CompanyId");
				if (companyIdClaim != null && int.TryParse(companyIdClaim.Value, out var cid))
				{
					await Groups.AddToGroupAsync(Context.ConnectionId, GetManagersGroup(cid));
				}
			}
		}

		await base.OnConnectedAsync();
	}

	// Утилитарные имена групп
	public static string GetCompanyGroup(int companyId) => $"company:{companyId}";
	public static string GetManagersGroup(int companyId) => $"managers:{companyId}";
	public static string GetProductGroup(int productId) => $"product:{productId}";

	// Оставляем публичный метод для вызова клиентских методов от имени hub (но серверу проще использовать IHubContext)
	public async Task SendPriceUpdate(int productId, decimal newPrice)
	{
		await Clients.All.SendAsync("ReceivePriceUpdate", productId, newPrice);
	}
}
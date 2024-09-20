using Microsoft.AspNetCore.SignalR;

public class PriceHub : Hub
{
	// Метод для отправки обновлений на клиент
	public async Task SendPriceUpdate(int productId, decimal newPrice)
	{
		await Clients.All.SendAsync("ReceivePriceUpdate", productId, newPrice);
	}
}
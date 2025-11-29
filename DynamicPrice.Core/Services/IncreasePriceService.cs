using DynamicPrice.Core.Data;
using DynamicPrice.Core.Models;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace DynamicPrice.Core.Services;

public class IncreasePriceService : IConsumer<PriceIncreaseEvent>
{
	private readonly IServiceProvider _serviceProvider;
	private readonly IHubContext<PriceHub> _priceHubContext;

	public IncreasePriceService(IServiceProvider serviceProvider, IHubContext<PriceHub> priceHubContext)
	{
		_serviceProvider = serviceProvider;
		_priceHubContext = priceHubContext;
	}

	public async Task Consume(ConsumeContext<PriceIncreaseEvent> context)
	{
		var msg = context.Message;

		if (msg != null)
		{
			await IncreasePrices(msg.OrderItems);
		}
	}

	private async Task IncreasePrices(IEnumerable<OrderItem> OrderItems)
	{
		using var scope = _serviceProvider.CreateScope();
		var context = scope.ServiceProvider.GetRequiredService<DynamicPriceCoreContext>();

		var companyId = OrderItems.FirstOrDefault()?.Product.CompanyId;

		var priceRule = await context.PriceRules
			.FirstOrDefaultAsync(p => p.Company.CompanyId == companyId);

		IncreasePrice(OrderItems, priceRule);

		await NoticeOfIncrease(OrderItems);

		context.SaveChanges();
	}

	private void IncreasePrice(IEnumerable<OrderItem> OrderItems, PriceRule priceRule)
	{
		foreach (var orderProduct in OrderItems)
		{
			var product = orderProduct.Product;
			var increase = product.Price * (decimal)priceRule.Increase * 0.01m * orderProduct.Quantity;
			product.Price += increase;
		}
	}
	private async Task NoticeOfIncrease(IEnumerable<OrderItem> OrderItems)
	{
		foreach (var orderProduct in OrderItems)
		{
			var product = orderProduct.Product;
			await _priceHubContext.Clients.All.SendAsync("ReceivePriceUpdate", product.ProductId, product.Price);
		}
	}
}

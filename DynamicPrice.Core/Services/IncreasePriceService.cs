using DynamicPrice.Core.Data;
using DynamicPrice.Core.Models;
using DynamicPrice.Core.SignalR;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace DynamicPrice.Core.Services;

//todo: think about base ChangePriceService and move common for Increase and Reduce
public class IncreasePriceService : IConsumer<PriceIncreaseEvent>
{
	private readonly IServiceProvider _serviceProvider;
	private readonly IHubContext<PriceHub> _priceHubContext;
	private readonly ILogger<IncreasePriceService> _logger;

	public IncreasePriceService(IServiceProvider serviceProvider, IHubContext<PriceHub> priceHubContext, ILogger<IncreasePriceService> logger)
	{
		_serviceProvider = serviceProvider;
		_priceHubContext = priceHubContext;
		_logger = logger;
	}

	public async Task Consume(ConsumeContext<PriceIncreaseEvent> context)
	{
		var msg = context.Message;

		if (msg != null)
		{
			try
			{
				await IncreasePrices(msg.OrderItems);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error processing PriceIncreaseEvent");
				throw;
			}
		}
	}

	private async Task IncreasePrices(IEnumerable<OrderItem> OrderItems)
	{
		using var scope = _serviceProvider.CreateScope();
		var context = scope.ServiceProvider.GetRequiredService<DynamicPriceCoreContext>();

		var companyId = OrderItems.FirstOrDefault()?.Product.CompanyId;
		var priceRule = await context.PriceRules
			.FirstOrDefaultAsync(p => p.Company.CompanyId == companyId);

		if (priceRule == null)
		{
			_logger.LogWarning("PriceRule not found for company {CompanyId}", companyId);
			return;
		}

		IncreasePrice(OrderItems, priceRule);

		await context.SaveChangesAsync();

		await NoticeOfIncrease(OrderItems);
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
		//todo: check
		foreach (var orderProduct in OrderItems)
		{
			var product = orderProduct.Product;
			try
			{
				await _priceHubContext.SendPriceUpdateToProductGroup(product.ProductId, product.Price);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Failed to notify hub about product {ProductId}", product.ProductId);
			}
		}
	}
}

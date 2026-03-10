using DynamicPrice.Core.Data;
using DynamicPrice.Core.Exceptions;
using DynamicPrice.Core.Models;
using DynamicPrice.Core.SignalR;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace DynamicPrice.Core.Services;

public abstract class PriceServiceBase<TEvent> : IConsumer<TEvent>
	where TEvent : class
{
	protected readonly IServiceProvider ServiceProvider;
	protected readonly IHubContext<PriceHub> PriceHubContext;
	protected readonly ILogger Logger;

	protected PriceServiceBase(
		IServiceProvider serviceProvider,
		IHubContext<PriceHub> priceHubContext,
		ILogger logger)
	{
		ServiceProvider = serviceProvider;
		PriceHubContext = priceHubContext;
		Logger = logger;
	}

	public async Task Consume(ConsumeContext<TEvent> context)
	{
		var msg = context.Message;

		if (msg == null)
			return;

		try
		{
			await ProcessMessage(msg);
		}
		catch (Exception ex)
		{
			Logger.LogError(ex, "Error processing price event");
			throw;
		}
	}

	protected abstract Task ProcessMessage(TEvent message);

	protected async Task UpdatePrice(
		int productId,
		Func<Product, PriceRule, decimal> priceCalculator)
	{
		using var scope = ServiceProvider.CreateScope();
		var context = scope.ServiceProvider.GetRequiredService<DynamicPriceCoreContext>();

		var product = await context.Products
			.FirstOrDefaultAsync(p => p.ProductId == productId);

		if (product == null)
			return;

		var priceRule = await context.PriceRules
			.FirstOrDefaultAsync(r => r.Company.CompanyId == product.CompanyId);

		if (priceRule == null)
			return;

		product.Price = priceCalculator(product, priceRule);

		await context.SaveChangesAsync();

		await PriceHubContext.SendPriceUpdateToProductGroup(product.ProductId, product.Price);
	}
}

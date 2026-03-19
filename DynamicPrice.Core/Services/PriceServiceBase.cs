using DynamicPrice.Core.Data;
using DynamicPrice.Core.Exceptions;
using DynamicPrice.Core.Models;
using DynamicPrice.Core.SignalR;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Prometheus;

namespace DynamicPrice.Core.Services;

public abstract class PriceServiceBase<TEvent> : IConsumer<TEvent>
	where TEvent : class
{
	protected readonly DynamicPriceCoreContext _context;
	protected readonly IHubContext<PriceHub> _priceHubContext;
	protected readonly ILogger _logger;

	private static readonly Histogram ChangePriceDuration = Metrics
		.CreateHistogram("dp_changeprice_duration_seconds",
			"Время обработки события изменения цены",
			new HistogramConfiguration
			{
				LabelNames = new[] { "companyId" }
			});


	protected PriceServiceBase(
		DynamicPriceCoreContext context,
		IHubContext<PriceHub> priceHubContext,
		ILogger logger)
	{
		_context = context;
		_priceHubContext = priceHubContext;
		_logger = logger;
	}

	public async Task Consume(ConsumeContext<TEvent> context)
	{
		var msg = context.Message
			?? throw new NotFoundException("Message not found.");

		try
		{
			await ProcessMessage(msg);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error processing price event");
			throw;
		}
	}

	protected abstract Task ProcessMessage(TEvent message);

	protected async Task UpdatePrice(
		int productId,
		Func<Product, PriceRule, decimal> priceCalculator)
	{
		var product = await _context.Products
			.FirstOrDefaultAsync(p => p.ProductId == productId)
			?? throw new NotFoundException("Product not found.");

		var priceRule = await _context.PriceRules
			.FirstOrDefaultAsync(pr => pr.Company.CompanyId == product.CompanyId)
			?? throw new NotFoundException("PriceRule not found.");

		var updatedPrice = priceCalculator(product, priceRule);

		if (product.Price != updatedPrice)
		{
			product.Price = updatedPrice;

			await _context.PriceDynamics.AddAsync(new PriceDynamic
			{
				ProductId = product.ProductId,
				Price = product.Price,
				Date = DateTime.UtcNow
			});

			await _context.SaveChangesAsync();

			var date = DateTime.UtcNow;

			await _priceHubContext.SendPriceUpdateToProductGroup(product.ProductId, product.Price);
		}
	}
}

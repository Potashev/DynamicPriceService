using DynamicPriceService.Data;
using Microsoft.EntityFrameworkCore;

namespace DynamicPriceService.Services;

/// <summary>
/// Сервис по снижению цен продуктов в "простое".
/// </summary>
public class ReducePriceService : BackgroundService
{
	private const int CONFIGURATION = 5000;

	private readonly IServiceScopeFactory _scopeFactory;
	private IActiveCompaniesService _activeCompaniesService;

	public ReducePriceService(IServiceScopeFactory serviceScopeFactory, IActiveCompaniesService activeCompaniesService)
	{
		_scopeFactory = serviceScopeFactory;
		_activeCompaniesService = activeCompaniesService;
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		using IServiceScope scope = _scopeFactory.CreateScope();
		var context = scope.ServiceProvider.GetService<DynamicPriceServiceContext>();

		while (!stoppingToken.IsCancellationRequested)
		{
			foreach(var company in _activeCompaniesService.GetActiveCompanies())
			{
				var priceRule = context.PriceRule.Where(pr => pr.Company == company).FirstOrDefault();

				var productsToReduceQuery = from p in context.Product where
										   p.Company == company &&
										   EF.Functions.DateDiffSecond(p.LastSellTime, DateTime.Now) > priceRule.NoSellTime.Value.TotalSeconds
										   select p;
				var productsToReduce = productsToReduceQuery.ToList();

				foreach (var product in productsToReduce)
				{
					product.Price = ReducePrice(product.Price, priceRule.Reduction);
				}

				context.SaveChanges();
			}

			await Task.Delay(CONFIGURATION, stoppingToken);
		}
		await Task.CompletedTask;
	}

	private double ReducePrice(double price, int pricingRuleReduction)
	{
		var reduction = pricingRuleReduction * 0.01 * price;
		price -= reduction;
		return price;
	}
}

using DynamicPriceService.Data;
using Microsoft.EntityFrameworkCore;
using Quartz;

namespace DynamicPriceService.Services;

public class ReducePriceJob : IJob
{
	private readonly DynamicPriceServiceContext _context;
	private IActiveCompaniesService _activeCompaniesService;

	public ReducePriceJob(DynamicPriceServiceContext context, IActiveCompaniesService activeCompaniesService)
	{
		_context = context;
		_activeCompaniesService = activeCompaniesService;
	}

	public Task Execute(IJobExecutionContext jobContext)
	{
		//todo: later - think about separate job for each company?
		foreach (var company in _activeCompaniesService.GetActiveCompanies())
		{
			var priceRule = _context.PriceRules.Where(pr => pr.Company == company).FirstOrDefault();

			var productsToReduceQuery = from p in _context.Products
										where
									   p.Company == company &&
									   EF.Functions.DateDiffSecond(p.LastSellTime, DateTime.Now) > priceRule.NoSellTime.Value.TotalSeconds
										select p;
			var productsToReduce = productsToReduceQuery.ToList();

			foreach (var product in productsToReduce)
			{
				product.Price = ReducePrice(product.Price, priceRule.Reduction);
			}

			_context.SaveChanges();
		}

		return Task.CompletedTask;
	}

	private double ReducePrice(double price, int pricingRuleReduction)
	{
		var reduction = pricingRuleReduction * 0.01 * price;
		price -= reduction;
		return price;
	}
}

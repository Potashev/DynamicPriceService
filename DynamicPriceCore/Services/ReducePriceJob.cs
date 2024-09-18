using DynamicPriceCore.Data;
using DynamicPriceCore.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Quartz;

namespace DynamicPriceCore.Services;

public class ReducePriceJob : IJob
{
	private readonly DynamicPriceCoreContext _context;
	private IActiveCompaniesService _activeCompaniesService;
	private readonly IHubContext<PriceHub> _priceHubContext;

	public ReducePriceJob(DynamicPriceCoreContext context, IActiveCompaniesService activeCompaniesService, IHubContext<PriceHub> priceHubContext)
	{
		_context = context;
		_activeCompaniesService = activeCompaniesService;
		_priceHubContext = priceHubContext;
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
									   EF.Functions.DateDiffSecond(p.LastSellTime, DateTime.UtcNow) > priceRule.NoSellTime.Value.TotalSeconds
										select p;
			var productsToReduce = productsToReduceQuery.ToList();

			foreach (var product in productsToReduce)
			{
				product.Price = ReducePrice(product.Price, priceRule.Reduction);
				if (product.Price < product.MinimumPrice)
					product.Price = product.MinimumPrice;

				var priceDynamic = new PriceDynamic
				{
					Product = product,
					Price = product.Price,
					Date = DateTime.UtcNow, //todo: make default
				};
				//_context.PriceDynamics.AddAsync(priceDynamic);

				_priceHubContext.Clients.All.SendAsync("ReceivePriceUpdate", product.ProductId, product.Price); //todo: make async?
			}

			_context.SaveChanges();
		}

		return Task.CompletedTask;
	}

	private decimal ReducePrice(decimal price, double pricingRuleReduction)
	{
		var reduction = (decimal)pricingRuleReduction * 0.01m * price; //todo: think about rounding
		price -= reduction;

		//todo: temp field for checking drawing - remove after test
		var maxrand = (int)Math.Round(reduction * 2);
		var rnd = new Random();
		price += rnd.Next(maxrand);

		return price;
	}
}

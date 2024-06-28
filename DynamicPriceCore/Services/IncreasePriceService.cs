using DynamicPriceCore.Data;
using DynamicPriceCore.Models;
using Microsoft.EntityFrameworkCore;

namespace DynamicPriceCore.Services;

public class IncreasePriceService : IIncreasePriceService
{
	private readonly DynamicPriceCoreContext _context;

    public IncreasePriceService(DynamicPriceCoreContext context)
		=> _context = context;

    public void Increase(IEnumerable<int> productIds)
	{
		var productsToIncrease = _context.Products
							.Include(p => p.Company)	//todo: make better
							.Where(p => productIds.Contains(p.ProductId))
							.ToList();
		var company = productsToIncrease.FirstOrDefault().Company;

		var priceRule = _context.PriceRules
			.FirstOrDefault(p => p.Company.CompanyId == company.CompanyId);

		foreach(var product in productsToIncrease)
		{
			product.Price = IncreasePrice(product.Price, priceRule.Increase);
		}
		_context.SaveChanges();
	}

	private double IncreasePrice(double productPrice, int pricingRuleIncrease)
	{
		var increase = pricingRuleIncrease * 0.01 * productPrice;
		productPrice += increase;
		return productPrice;
	}
}

public interface IIncreasePriceService
{
	void Increase(IEnumerable<int> productIds);
}

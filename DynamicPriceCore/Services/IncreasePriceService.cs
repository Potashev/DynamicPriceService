using DynamicPriceCore.Data;
using DynamicPriceCore.Models;
using Microsoft.EntityFrameworkCore;

namespace DynamicPriceCore.Services;

public class IncreasePriceService : IIncreasePriceService
{
	private readonly DynamicPriceCoreContext _context;

    public IncreasePriceService(DynamicPriceCoreContext context)
		=> _context = context;

    public void Increase(IEnumerable<OrderProduct> OrderProducts)
	{
		var company = _context.Products
			.Where(p => p.ProductId == OrderProducts.FirstOrDefault().ProductId)
			.Select(p => p.Company).FirstOrDefault();		

		var priceRule = _context.PriceRules
			.FirstOrDefault(p => p.Company.CompanyId == company.CompanyId);

		IncreasePrice(OrderProducts, priceRule);

		_context.SaveChanges();
	}

	private void IncreasePrice(IEnumerable<OrderProduct> OrderProducts, PriceRule priceRule)
	{
		foreach(var orderProduct in OrderProducts)
		{
			var product = orderProduct.Product;
			var increase = product.Price * priceRule.Increase * 0.01 * orderProduct.Quantity;
			product.Price += increase;
		}
	}
}

public interface IIncreasePriceService
{
	void Increase(IEnumerable<OrderProduct> OrderProducts);
}

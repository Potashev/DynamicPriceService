using DynamicPriceCore.Data;
using DynamicPriceCore.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace DynamicPriceCore.Services;

public class IncreasePriceService : IIncreasePriceService
{
	private readonly DynamicPriceCoreContext _context;
	private readonly IHubContext<PriceHub> _priceHubContext;

	public IncreasePriceService(DynamicPriceCoreContext context, IHubContext<PriceHub> priceHubContext)
	{
		_context = context;
		_priceHubContext = priceHubContext;
	}

    public async Task Increase(IEnumerable<OrderProduct> OrderProducts)
	{
		var company = _context.Products
			.Where(p => p.ProductId == OrderProducts.FirstOrDefault().ProductId)
			.Select(p => p.Company).FirstOrDefault();		

		var priceRule = _context.PriceRules
			.FirstOrDefault(p => p.Company.CompanyId == company.CompanyId);

		IncreasePrice(OrderProducts, priceRule);

		await NoticeOfIncrease(OrderProducts);

		_context.SaveChanges();
	}

	private void IncreasePrice(IEnumerable<OrderProduct> OrderProducts, PriceRule priceRule)
	{
		foreach(var orderProduct in OrderProducts)
		{
			var product = orderProduct.Product;
			var increase = product.Price * (decimal)priceRule.Increase * 0.01m * orderProduct.Quantity;
			product.Price += increase;
		}
	}
	private async Task NoticeOfIncrease(IEnumerable<OrderProduct> OrderProducts)
	{
		foreach(var orderProduct in OrderProducts)
		{
			var product = orderProduct.Product;
			await _priceHubContext.Clients.All.SendAsync("ReceivePriceUpdate", product.ProductId, product.Price);
		}
	}
}

public interface IIncreasePriceService
{
	Task Increase(IEnumerable<OrderProduct> OrderProducts);
}

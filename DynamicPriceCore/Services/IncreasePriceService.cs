using DynamicPriceCore.Data;
using DynamicPriceCore.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.CodeAnalysis;

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

	//todo: check replacing OrderProduct with OrderItem
	public async Task Increase(IEnumerable<OrderItem> OrderItems)
	{
		var company = _context.Products
			.Where(p => p.ProductId == OrderItems.FirstOrDefault().ProductId)
			.Select(p => p.Company).FirstOrDefault();

		var priceRule = _context.PriceRules
			.FirstOrDefault(p => p.Company.CompanyId == company.CompanyId);

		IncreasePrice(OrderItems, priceRule);

		await NoticeOfIncrease(OrderItems);

		_context.SaveChanges();
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

public interface IIncreasePriceService
{
	Task Increase(IEnumerable<OrderItem> OrderItems);
}

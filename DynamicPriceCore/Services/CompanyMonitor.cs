using DynamicPriceCore.Data;
using Microsoft.EntityFrameworkCore;

namespace DynamicPrice.Core.Services;

public class CompanyMonitor
{
	private readonly DynamicPriceCoreContext _context;

	public CompanyMonitor(DynamicPriceCoreContext context)
	{
		_context = context;
	}

	public async Task<List<int>> FindProductsToReduceAsync(int companyId, CancellationToken token)
	{
		var priceRule = await _context.PriceRules
			.Where(pr => pr.Company.CompanyId == companyId)
			.FirstOrDefaultAsync(token);

		if (priceRule == null) return new List<int>();

		Thread.Sleep(10);

		return await _context.Products
			.Where(p =>
				p.Company.CompanyId == companyId &&
				EF.Functions.DateDiffSecond(p.LastSellTime, DateTime.UtcNow) >
				priceRule.NoSellTime.Value.TotalSeconds)
			.Select(p => p.ProductId)
			.ToListAsync(token);

	}
}


using DynamicPriceCore.Data;
using Microsoft.EntityFrameworkCore;

namespace DynamicPrice.Core.Services;

//todo: obsolete
public class CompanyMonitor
{
	private readonly DynamicPriceCoreContext _context;

	public CompanyMonitor(DynamicPriceCoreContext context)
	{
		_context = context;
	}

	public async Task<List<int>> FindProductsToReduceAsync(
		int companyId,
		CancellationToken token,
		int? productsCount = null)
	{
		var priceRule = await _context.PriceRules
			.Where(pr => pr.Company.CompanyId == companyId)
			.FirstOrDefaultAsync(token);

		if (priceRule == null)
			return new List<int>();

		var query = _context.Products.AsQueryable();

		if (productsCount.HasValue)
			query = query.Take(productsCount.Value);

		query = query.Where(p =>
			p.Company.CompanyId == companyId &&
			EF.Functions.DateDiffSecond(p.LastSellTime, DateTime.UtcNow) >
			priceRule.NoSellTime.Value.TotalSeconds);

		return await query
			.Select(p => p.ProductId)
			.ToListAsync(token);
	}

}

using DynamicPriceService.Data;
using DynamicPriceService.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DynamicPriceService.MediatR.PriceRuleEntity.Queries;

public class GetPriceRuleDetailsQueryHandler
	: IRequestHandler<GetPriceRuleDetailsQuery, PriceRule>
{
	private readonly DynamicPriceServiceContext _context;

    public GetPriceRuleDetailsQueryHandler(DynamicPriceServiceContext context)
        => _context = context;

	public async Task<PriceRule> Handle(GetPriceRuleDetailsQuery request, CancellationToken cancellationToken)
	{
		var company = request.Company;
		var priceRule = await _context.PriceRule
				.FirstOrDefaultAsync(pr => pr.PriceRuleId == company.CompanyId);

		priceRule ??= AddDefaultRule(company);

		return priceRule;
	}

	private PriceRule AddDefaultRule(Company company)
	{
		var priceRule = new PriceRule
		{
			Increase = 10,
			Reduction = 1,
			NoSellTime = new TimeSpan(0, 0, 10),
			Company = company
		};

		// concept conflict - write action in query
		_context.Add(priceRule);
		_context.SaveChanges();

		//PriceRuleId lost
		return priceRule;
	}

}

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
        var company = await _context.CompanyUsers
            .Where(cu => cu.UserId == request.UserId)
            .Select(cu => cu.Company)
            .FirstOrDefaultAsync();

        var priceRule = await _context.PriceRules
			.FirstOrDefaultAsync(pr => pr.Company.CompanyId == company.CompanyId);

        //todo: when priceRule not exist for the company - do we need add default rule?
		return priceRule;
	}
}

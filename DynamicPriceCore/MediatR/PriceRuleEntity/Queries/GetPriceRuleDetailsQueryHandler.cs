using AutoMapper;
using DynamicPriceCore.Data;
using DynamicPriceCore.MediatR.ViewModels;
using DynamicPriceCore.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DynamicPriceCore.MediatR.PriceRuleEntity.Queries;

public class GetPriceRuleDetailsQueryHandler
	: IRequestHandler<GetPriceRuleDetailsQuery, PriceRuleViewModel>
{
	private readonly DynamicPriceCoreContext _context;
    private readonly IMapper _mapper;

    public GetPriceRuleDetailsQueryHandler(DynamicPriceCoreContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

	public async Task<PriceRuleViewModel> Handle(GetPriceRuleDetailsQuery request, CancellationToken cancellationToken)
	{
        var company = await _context.CompanyUsers
            .Where(cu => cu.UserId == request.UserId)
            .Select(cu => cu.Company)
            .FirstOrDefaultAsync();

        PriceRule priceRule = new PriceRule();

        try
        {
            priceRule = await _context.PriceRules
                .FirstOrDefaultAsync(pr => pr.Company.CompanyId == company.CompanyId);
        }
        catch(Exception ex)
        {

        }

        var priceRuleVm = _mapper.Map<PriceRuleViewModel>(priceRule);

        //todo: when priceRule not exist for the company - do we need add default rule?
		return priceRuleVm;
	}
}

using AutoMapper;
using DynamicPriceCore.Data;
using DynamicPriceCore.MediatR.ViewModels;
using DynamicPriceCore.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DynamicPriceCore.MediatR.PriceRuleEntity.Queries;

public class GetPriceRuleWithStatusQueryHandler
	: IRequestHandler<GetPriceRuleWithStatusQuery, PriceRuleWithStatus>
{
    private IMapper _mapper;
    private DynamicPriceCoreContext _context;
    private IActiveCompaniesService _activeCompaniesService;

	public GetPriceRuleWithStatusQueryHandler(IMapper mapper, DynamicPriceCoreContext context, IActiveCompaniesService activeCompaniesService)
		=> (_mapper, _context, _activeCompaniesService) = (mapper, context, activeCompaniesService);
    public async Task<PriceRuleWithStatus> Handle(GetPriceRuleWithStatusQuery request, CancellationToken cancellationToken)
    {
        var priceRule = await _context.PriceRules
                       .Include(pr => pr.Company)
                       .Where(pr => pr.Company.CompanyUsers
                                       .Any(cu => cu.UserId == request.UserId))
                       .FirstOrDefaultAsync();

        var priceRuleVm = _mapper.Map<PriceRuleViewModel>(priceRule);
        var status = _activeCompaniesService.IsActive(priceRule.Company);

        return new PriceRuleWithStatus(priceRuleVm, status);
    }
}

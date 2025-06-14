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
	private readonly ICurrentUserService _currentUserService;

	public GetPriceRuleWithStatusQueryHandler(IMapper mapper, DynamicPriceCoreContext context, IActiveCompaniesService activeCompaniesService, ICurrentUserService currentUserService)
		=> (_mapper, _context, _activeCompaniesService, _currentUserService) = (mapper, context, activeCompaniesService, currentUserService);
	public async Task<PriceRuleWithStatus> Handle(GetPriceRuleWithStatusQuery request, CancellationToken cancellationToken)
	{
		var manager = await _currentUserService.GetCurrentUserAsync();

		var priceRule = await _context.PriceRules
			   .Where(pr => pr.Company.CompanyId == manager.CompanyId)
			   .FirstOrDefaultAsync(cancellationToken);

		var priceRuleVm = _mapper.Map<PriceRuleViewModel>(priceRule);
		var status = _activeCompaniesService.IsActive(priceRule.Company);

		return new PriceRuleWithStatus(priceRuleVm, status);
	}
}

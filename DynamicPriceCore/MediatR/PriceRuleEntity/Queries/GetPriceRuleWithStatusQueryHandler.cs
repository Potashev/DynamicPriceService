using AutoMapper;
using DynamicPriceCore.Data;
using DynamicPriceCore.Services;
using DynamicPriceCore.ViewModels;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DynamicPriceCore.MediatR.PriceRuleEntity.Queries;

public class GetPriceRuleWithStatusQueryHandler
	: IRequestHandler<GetPriceRuleWithStatusQuery, PriceRuleWithStatus>
{
	private IMapper _mapper;
	private DynamicPriceCoreContext _context;
	private IActiveCompaniesService _activeCompaniesService;
	private readonly IUserService _userService;

	public GetPriceRuleWithStatusQueryHandler(IMapper mapper, DynamicPriceCoreContext context, IActiveCompaniesService activeCompaniesService, IUserService userService)
		=> (_mapper, _context, _activeCompaniesService, _userService) = (mapper, context, activeCompaniesService, userService);
	public async Task<PriceRuleWithStatus> Handle(GetPriceRuleWithStatusQuery request, CancellationToken cancellationToken)
	{
		var manager = await _userService.GetCurrentUserAsync();

		var priceRule = await _context.PriceRules
			.Where(pr => pr.Company.CompanyId == manager.CompanyId)
			.FirstOrDefaultAsync(cancellationToken);

		var priceRuleVm = _mapper.Map<PriceRuleViewModel>(priceRule);
		var status = _activeCompaniesService.IsActive((int)priceRule.CompanyId);

		return new PriceRuleWithStatus(priceRuleVm, status);
	}
}

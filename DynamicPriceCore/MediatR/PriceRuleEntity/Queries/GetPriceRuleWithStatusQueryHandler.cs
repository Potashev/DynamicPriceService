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
	private readonly IMapper _mapper;
	private readonly DynamicPriceCoreContext _context;
	//private IActiveCompaniesService _activeCompaniesService;
	private readonly IUserService _userService;

	public GetPriceRuleWithStatusQueryHandler(IMapper mapper, DynamicPriceCoreContext context, IUserService userService)
		=> (_mapper, _context, _userService) = (mapper, context, userService);
	public async Task<PriceRuleWithStatus> Handle(GetPriceRuleWithStatusQuery request, CancellationToken cancellationToken)
	{
		var manager = await _userService.GetCurrentUserAsync();

		var priceRule = await _context.PriceRules
			.Where(pr => pr.Company.CompanyId == manager.CompanyId)
			.FirstOrDefaultAsync(cancellationToken);

		var priceRuleVm = _mapper.Map<PriceRuleViewModel>(priceRule);

		var status = priceRule?.CompanyId is int cid
			&& await _context.ActiveCompanies
				.AsNoTracking()
				.AnyAsync(ac => ac.CompanyId == cid, cancellationToken);

		return new PriceRuleWithStatus(priceRuleVm, status);
	}
}

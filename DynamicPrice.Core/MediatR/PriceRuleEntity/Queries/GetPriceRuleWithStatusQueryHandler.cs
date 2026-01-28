using AutoMapper;
using DynamicPrice.Core.Data;
using DynamicPrice.Core.Services;
using DynamicPrice.Shared.Contracts.ViewModels;
using DynamicPrice.Shared.Contracts.ViewModels.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DynamicPrice.Core.MediatR.PriceRuleEntity.Queries;

public class GetPriceRuleWithStatusQueryHandler
	: IRequestHandler<GetPriceRuleWithStatusQuery, PriceRuleWithStatus>
{
	private readonly IMapper _mapper;
	private readonly DynamicPriceCoreContext _context;
	private readonly IUserService _userService;

	public GetPriceRuleWithStatusQueryHandler(
		DynamicPriceCoreContext context,
		IMapper mapper,
		IUserService userService)
		=> (_mapper, _context, _userService) = (mapper, context, userService);
	public async Task<PriceRuleWithStatus> Handle(
		GetPriceRuleWithStatusQuery request,
		CancellationToken cancellationToken)
	{
		var manager = await _userService.GetRequiredCurrentUserAsync();

		var priceRule = await _context.PriceRules
			.Where(pr => pr.Company.CompanyId == manager.CompanyId)
			.FirstOrDefaultAsync(cancellationToken);

		var priceRuleVm = _mapper.Map<PriceRuleViewModel>(priceRule);

		var status = priceRule?.CompanyId is int cid
			&& await _context.ActiveCompanies
				.AsNoTracking()
				.AnyAsync(ac => ac.CompanyId == cid, cancellationToken);

		return new PriceRuleWithStatus
		{
			PriceRule = priceRuleVm,
			IsActive = status
		};
	}
}

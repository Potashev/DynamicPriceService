using AutoMapper;
using DynamicPrice.Core.Data;
using DynamicPrice.Core.Exceptions;
using DynamicPrice.Core.Services;
using DynamicPrice.Shared.Contracts.ViewModels;
using DynamicPrice.Shared.Contracts.ViewModels.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DynamicPrice.Core.MediatR.PriceRuleEntity.Queries;

public class GetPriceRuleWithStatusQueryHandler(
	DynamicPriceCoreContext context,
	IMapper mapper,
	IUserService userService)
	: IRequestHandler<GetPriceRuleWithStatusQuery, PriceRuleWithStatus>
{
	public async Task<PriceRuleWithStatus> Handle(
		GetPriceRuleWithStatusQuery request,
		CancellationToken cancellationToken)
	{
		var manager = await userService.GetRequiredCurrentUserAsync();

		var priceRule = await context.PriceRules
			.Where(pr => pr.CompanyId == manager.CompanyId)
			.FirstOrDefaultAsync(cancellationToken);

		var priceRuleVm = mapper.Map<PriceRuleViewModel>(priceRule);

		var status = priceRule?.CompanyId is Guid cid
			&& await context.ActiveCompanies
				.AsNoTracking()
				.AnyAsync(ac => ac.Id == cid, cancellationToken);

		return new PriceRuleWithStatus
		{
			PriceRule = priceRuleVm,
			IsActive = status
		};
	}
}

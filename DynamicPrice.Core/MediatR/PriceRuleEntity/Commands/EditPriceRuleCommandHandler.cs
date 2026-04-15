using AutoMapper;
using DynamicPrice.Core.Data;
using DynamicPrice.Core.Exceptions;
using DynamicPrice.Core.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DynamicPrice.Core.MediatR.PriceRuleEntity.Commands;

public class EditPriceRuleCommandHandler(
	DynamicPriceCoreContext context,
	IMapper mapper,
	IUserService userService)
	: IRequestHandler<EditPriceRuleCommand, int>
{
	public async Task<int> Handle(
		EditPriceRuleCommand request,
		CancellationToken cancellationToken)
	{
		var manager = await userService.GetRequiredCurrentUserAsync();

		var updatedPriceRuleVm = request.PriceRuleVm;
		var priceRule = await context.PriceRules
			.FirstOrDefaultAsync(pr => 
				pr.PriceRuleId == updatedPriceRuleVm.PriceRuleId && 
				pr.Company.CompanyId == manager.CompanyId, cancellationToken)
			?? throw new NotFoundException("Price rule not found.");

		mapper.Map(updatedPriceRuleVm, priceRule);

		context.Update(priceRule);
		await context.SaveChangesAsync(cancellationToken);

		return priceRule.PriceRuleId;
	}
}

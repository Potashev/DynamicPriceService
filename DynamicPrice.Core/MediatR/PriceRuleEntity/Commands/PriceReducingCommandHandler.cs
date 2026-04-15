using DynamicPrice.Core.Data;
using DynamicPrice.Core.Exceptions;
using DynamicPrice.Core.Models;
using DynamicPrice.Core.Services;
using MediatR;

namespace DynamicPrice.Core.MediatR.PriceRuleEntity.Commands;

public class PriceReducingCommandHandler(
	DynamicPriceCoreContext context,
	IUserService userService)
	: IRequestHandler<PriceReducingCommand>
{
	public async Task Handle(
		PriceReducingCommand request,
		CancellationToken cancellationToken)
	{
		var manager = await userService.GetRequiredCurrentUserAsync();

		var companyId = manager.CompanyId
			?? throw new BusinessException("Manager is not assigned to a company.");

		var activeCompany = await context.ActiveCompanies.FindAsync(companyId);

		if (request.IsRunCommand && activeCompany is null)
			context.ActiveCompanies.Add(new ActiveCompany { CompanyId = companyId, StartedAt = DateTime.UtcNow });
		else if (!request.IsRunCommand && activeCompany is not null)
			context.ActiveCompanies.Remove(activeCompany);

		await context.SaveChangesAsync(cancellationToken);

		return;
	}
}

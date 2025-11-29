using DynamicPrice.Core.Data;
using DynamicPrice.Core.Models;
using DynamicPrice.Core.Services;
using MediatR;

namespace DynamicPrice.Core.MediatR.PriceRuleEntity.Commands;

public class PriceReducingCommandHandler
	: IRequestHandler<PriceReducingCommand>
{
	private readonly IUserService _userService;
	private readonly DynamicPriceCoreContext _context;

	public PriceReducingCommandHandler(IUserService userService, DynamicPriceCoreContext context)
		=> (_userService, _context) = (userService, context);

	public async Task Handle(PriceReducingCommand request, CancellationToken cancellationToken)
	{
		var manager = await _userService.GetCurrentUserAsync();

		var companyId = (int)manager.CompanyId;

		var activeCompany = await _context.ActiveCompanies.FindAsync(companyId);

		if (request.IsRunCommand && activeCompany is null)
			//todo: set lastmonitoring as default?
			_context.ActiveCompanies.Add(new ActiveCompany { CompanyId = companyId, StartedAt = DateTime.UtcNow });
		else if (!request.IsRunCommand && activeCompany is not null)
			_context.ActiveCompanies.Remove(activeCompany);

		await _context.SaveChangesAsync(cancellationToken);

		return;
	}
}

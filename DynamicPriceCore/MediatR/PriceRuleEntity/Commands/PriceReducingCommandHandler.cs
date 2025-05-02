using DynamicPriceCore.Data;
using DynamicPriceCore.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DynamicPriceCore.MediatR.PriceRuleEntity.Commands;

public class PriceReducingCommandHandler
	: IRequestHandler<PriceReducingCommand, bool>
{
	private DynamicPriceCoreContext _context;
	private IActiveCompaniesService _activeCompaniesService;
	private readonly ICurrentUserService _currentUserService;

	public PriceReducingCommandHandler(DynamicPriceCoreContext context, IActiveCompaniesService activeCompaniesService, ICurrentUserService currentUserService)
		=> (_context, _activeCompaniesService, _currentUserService) = (context, activeCompaniesService, currentUserService);

	public async Task<bool> Handle(PriceReducingCommand request, CancellationToken cancellationToken)
	{
		//var company = await _context.CompanyUsers
		//	.Where(cu => cu.UserId == request.UserId)
		//	.Select(cu => cu.Company)
		//	.FirstOrDefaultAsync(cancellationToken);

		var manager = await _currentUserService.GetCurrentManagerAsync();
		var company = manager.Company;

		if (request.IsRunCommand)
			_activeCompaniesService.Add(company);
		else
			_activeCompaniesService.Remove(company);

		return _activeCompaniesService.IsActive(company);
	}
}

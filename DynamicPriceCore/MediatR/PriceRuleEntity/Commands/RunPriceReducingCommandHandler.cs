using DynamicPriceCore.Data;
using DynamicPriceCore.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DynamicPriceCore.MediatR.PriceRuleEntity.Commands;

public class RunPriceReducingCommandHandler
	: IRequestHandler<RunPriceReducingCommand, bool>
{
	private DynamicPriceCoreContext _context;
	private IActiveCompaniesService _activeCompaniesService;

	public RunPriceReducingCommandHandler(DynamicPriceCoreContext context, IActiveCompaniesService activeCompaniesService)
		=> (_context, _activeCompaniesService) = (context, activeCompaniesService);

	public async Task<bool> Handle(RunPriceReducingCommand request, CancellationToken cancellationToken)
	{
		var company = await _context.CompanyUsers
			.Where(cu => cu.UserId == request.UserId)
			.Select(cu => cu.Company)
			.FirstOrDefaultAsync();

		_activeCompaniesService.Add(company);
		return _activeCompaniesService.IsActive(company);
	}
}

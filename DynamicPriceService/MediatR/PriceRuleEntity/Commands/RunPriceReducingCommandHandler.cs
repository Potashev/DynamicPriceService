using DynamicPriceService.Services;
using MediatR;

namespace DynamicPriceService.MediatR.PriceRuleEntity.Commands;

public class RunPriceReducingCommandHandler
	: IRequestHandler<RunPriceReducingCommand, bool>
{
	private IActiveCompaniesService _activeCompaniesService;

	public RunPriceReducingCommandHandler(IActiveCompaniesService activeCompaniesService)
		=> _activeCompaniesService = activeCompaniesService;

	public async Task<bool> Handle(RunPriceReducingCommand request, CancellationToken cancellationToken)
	{
		var company = request.Company;
		_activeCompaniesService.Add(company);
		return _activeCompaniesService.IsActive(company);
	}
}

using DynamicPriceService.Services;
using MediatR;

namespace DynamicPriceService.MediatR.PriceRuleEntity.Queries;

public class GetPriceRuleStatusQueryHandler
	: IRequestHandler<GetPriceRuleStatusQuery, bool>
{
	private IActiveCompaniesService _activeCompaniesService;

	public GetPriceRuleStatusQueryHandler(IActiveCompaniesService activeCompaniesService)
		=> _activeCompaniesService = activeCompaniesService;

	public async Task<bool> Handle(GetPriceRuleStatusQuery request, CancellationToken cancellationToken)
		=> _activeCompaniesService.IsActive(request.Company);
}

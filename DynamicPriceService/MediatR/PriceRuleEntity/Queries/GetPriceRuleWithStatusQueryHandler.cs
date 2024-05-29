using DynamicPriceService.Services;
using MediatR;

namespace DynamicPriceService.MediatR.PriceRuleEntity.Queries;

public class GetPriceRuleWithStatusQueryHandler
	: IRequestHandler<GetPriceRuleWithStatusQuery, PriceRuleWithStatusDto>
{
    private IMediator _mediator;
    private IActiveCompaniesService _activeCompaniesService;

	public GetPriceRuleWithStatusQueryHandler(IMediator mediator, IActiveCompaniesService activeCompaniesService)
		=> (_mediator, _activeCompaniesService) = (mediator, activeCompaniesService);
    public async Task<PriceRuleWithStatusDto> Handle(GetPriceRuleWithStatusQuery request, CancellationToken cancellationToken)
    {
        var company = request.Company;
        var priceRule = await _mediator.Send(new GetPriceRuleDetailsQuery(company));
        var status = _activeCompaniesService.IsActive(company);

        return new PriceRuleWithStatusDto(priceRule, status);
    }
}

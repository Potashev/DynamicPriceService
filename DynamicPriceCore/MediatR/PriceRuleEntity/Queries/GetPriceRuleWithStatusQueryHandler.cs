using DynamicPriceCore.Data;
using DynamicPriceCore.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DynamicPriceCore.MediatR.PriceRuleEntity.Queries;

public class GetPriceRuleWithStatusQueryHandler
	: IRequestHandler<GetPriceRuleWithStatusQuery, PriceRuleWithStatusDto>
{
    private IMediator _mediator;
    private DynamicPriceCoreContext _context;
    private IActiveCompaniesService _activeCompaniesService;

	public GetPriceRuleWithStatusQueryHandler(IMediator mediator, DynamicPriceCoreContext context, IActiveCompaniesService activeCompaniesService)
		=> (_mediator, _context, _activeCompaniesService) = (mediator, context, activeCompaniesService);
    public async Task<PriceRuleWithStatusDto> Handle(GetPriceRuleWithStatusQuery request, CancellationToken cancellationToken)
    {
        var company = await _context.CompanyUsers
            .Where(cu => cu.UserId == request.UserId)
            .Select(cu => cu.Company)
            .FirstOrDefaultAsync();

        var priceRule = await _mediator.Send(new GetPriceRuleDetailsQuery(request.UserId));
        var status = _activeCompaniesService.IsActive(company);

        return new PriceRuleWithStatusDto(priceRule, status);
    }
}

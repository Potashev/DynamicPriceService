using DynamicPriceCore.Data;
using DynamicPriceCore.MediatR.ViewModels;
using DynamicPriceCore.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DynamicPriceCore.MediatR.PriceRuleEntity.Queries;

public class GetPriceRuleWithStatusQueryHandler
	: IRequestHandler<GetPriceRuleWithStatusQuery, PriceRuleWithStatus>
{
    private IMediator _mediator;
    private DynamicPriceCoreContext _context;
    private IActiveCompaniesService _activeCompaniesService;

	public GetPriceRuleWithStatusQueryHandler(IMediator mediator, DynamicPriceCoreContext context, IActiveCompaniesService activeCompaniesService)
		=> (_mediator, _context, _activeCompaniesService) = (mediator, context, activeCompaniesService);
    public async Task<PriceRuleWithStatus> Handle(GetPriceRuleWithStatusQuery request, CancellationToken cancellationToken)
    {
        var company = await _context.CompanyUsers
            .Where(cu => cu.UserId == request.UserId)
            .Select(cu => cu.Company)
            .FirstOrDefaultAsync();

        var priceRuleVm = await _mediator.Send(new GetPriceRuleDetailsQuery(request.UserId));

        //var priceRule = await _context.PriceRules
        //               .Include(pr => pr.Company)
        //               .Where(pr => pr.Company.CompanyUsers
        //                               .Any(cu => cu.UserId == request.UserId))
        //               .FirstOrDefaultAsync();

        //var priceRule = await _context.CompanyUsers
        //                .Include(cu => cu.Company)
        //                .Include()


        var status = _activeCompaniesService.IsActive(company);

        return new PriceRuleWithStatus(priceRuleVm, status);
    }
}

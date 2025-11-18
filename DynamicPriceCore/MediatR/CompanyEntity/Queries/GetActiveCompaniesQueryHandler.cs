using AutoMapper;
using DynamicPriceCore.Data;
using DynamicPriceCore.Services;
using DynamicPriceCore.ViewModels;
using MediatR;

namespace DynamicPriceCore.MediatR.CompanyEntity.Queries;

public class GetActiveCompaniesQueryHandler
	: IRequestHandler<GetActiveCompaniesQuery, IEnumerable<CompanyViewModel>>
{
    //private readonly IActiveCompaniesService _activeCompaniesService;
    private readonly DynamicPriceCoreContext _context;
    private readonly IMapper _mapper;

	public GetActiveCompaniesQueryHandler(DynamicPriceCoreContext context, IMapper mapper)
		=> (_context, _mapper) = (context, mapper);

    public async Task<IEnumerable<CompanyViewModel>> Handle(GetActiveCompaniesQuery request, CancellationToken cancellationToken)
        => _mapper.Map<CompanyViewModel[]>(_context.ActiveCompanies.Select(ac => ac.Company).ToList());
}

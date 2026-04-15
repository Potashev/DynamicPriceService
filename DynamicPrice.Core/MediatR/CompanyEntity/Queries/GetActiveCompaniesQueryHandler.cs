using AutoMapper;
using DynamicPrice.Core.Data;
using DynamicPrice.Core.Exceptions;
using DynamicPrice.Shared.Contracts.ViewModels;
using MediatR;

namespace DynamicPrice.Core.MediatR.CompanyEntity.Queries;

public class GetActiveCompaniesQueryHandler(
	DynamicPriceCoreContext context,
	IMapper mapper)
	: IRequestHandler<GetActiveCompaniesQuery, IEnumerable<CompanyViewModel>>
{
	public async Task<IEnumerable<CompanyViewModel>> Handle(
		GetActiveCompaniesQuery request,
		CancellationToken cancellationToken)
		=> mapper.Map<CompanyViewModel[]>(context.ActiveCompanies.Select(ac => ac.Company).ToList());
}

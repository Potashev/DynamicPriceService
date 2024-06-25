using AutoMapper;
using DynamicPriceCore.Data;
using DynamicPriceCore.MediatR.ProductEntity.Queries;
using DynamicPriceCore.Models;
using DynamicPriceCore.Services;
using MediatR;

namespace DynamicPriceCore.MediatR.CompanyEntity.Queries;

public class GetActiveCompaniesQueryHandler
	: IRequestHandler<GetActiveCompaniesQuery, IEnumerable<Company>>
{
	private readonly IActiveCompaniesService _activeCompaniesService;
	//private readonly IMapper _mapper;

	public GetActiveCompaniesQueryHandler(IActiveCompaniesService activeCompaniesService)
		=> _activeCompaniesService = activeCompaniesService;

	public async Task<IEnumerable<Company>> Handle(GetActiveCompaniesQuery request, CancellationToken cancellationToken)
		=> _activeCompaniesService.GetActiveCompanies();
}

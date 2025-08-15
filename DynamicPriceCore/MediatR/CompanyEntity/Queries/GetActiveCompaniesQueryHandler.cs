using AutoMapper;
using DynamicPriceCore.Models;
using DynamicPriceCore.Services;
using DynamicPriceCore.ViewModels;
using MediatR;

namespace DynamicPriceCore.MediatR.CompanyEntity.Queries;

public class GetActiveCompaniesQueryHandler
	: IRequestHandler<GetActiveCompaniesQuery, IEnumerable<CompanyViewModel>>
{
	private readonly IActiveCompaniesService _activeCompaniesService;
	private readonly IMapper _mapper;

	public GetActiveCompaniesQueryHandler(IActiveCompaniesService activeCompaniesService, IMapper mapper)
		=> (_activeCompaniesService, _mapper) = (activeCompaniesService, mapper);

	//public async Task<IEnumerable<CompanyViewModel>> Handle(GetActiveCompaniesQuery request, CancellationToken cancellationToken)
	//	=> _mapper.Map<CompanyViewModel[]>(_activeCompaniesService.GetActiveCompanies());

	public async Task<IEnumerable<CompanyViewModel>> Handle(GetActiveCompaniesQuery request, CancellationToken cancellationToken)
	=> _mapper.Map<CompanyViewModel[]>(_activeCompaniesService.GetActiveCompanies());
}

using AutoMapper;
using DynamicPrice.Core.Services;
using DynamicPrice.Shared.Contracts.ViewModels.Responses;
using MediatR;

namespace DynamicPrice.Core.MediatR.ManagerEntity.Queries;

public class GetCompanyManagersQueryHandler
	: IRequestHandler<GetCompanyManagersQuery, IEnumerable<ManagerInfoViewModel>>
{
	private readonly IMapper _mapper;
	private readonly IUserService _userService;

	public GetCompanyManagersQueryHandler(
		IMapper mapper,
		IUserService userService)
		=> (_mapper, _userService) = (mapper, userService);

	public async Task<IEnumerable<ManagerInfoViewModel>> Handle(
		GetCompanyManagersQuery request,
		CancellationToken cancellationToken)
	{
		var manager = await _userService.GetRequiredCurrentUserAsync();

		var companyManagers = await _userService.GetUsersAsync(au => au.CompanyId == manager.CompanyId);


		return _mapper.Map<ManagerInfoViewModel[]>(companyManagers);
	}
}

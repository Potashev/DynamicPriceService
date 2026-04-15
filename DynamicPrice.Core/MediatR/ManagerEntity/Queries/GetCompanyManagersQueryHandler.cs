using AutoMapper;
using DynamicPrice.Core.Services;
using DynamicPrice.Shared.Contracts.ViewModels.Responses;
using MediatR;

namespace DynamicPrice.Core.MediatR.ManagerEntity.Queries;

public class GetCompanyManagersQueryHandler(
	IMapper mapper,
	IUserService userService)
	: IRequestHandler<GetCompanyManagersQuery, IEnumerable<ManagerInfoViewModel>>
{
	public async Task<IEnumerable<ManagerInfoViewModel>> Handle(
		GetCompanyManagersQuery request,
		CancellationToken cancellationToken)
	{
		var manager = await userService.GetRequiredCurrentUserAsync();

		var companyManagers = await userService.GetUsersAsync(au => au.CompanyId == manager.CompanyId);


		return mapper.Map<ManagerInfoViewModel[]>(companyManagers);
	}
}

using AutoMapper;
using DynamicPrice.Core.Data;
using DynamicPrice.Core.Exceptions;
using DynamicPrice.Core.Services;
using DynamicPrice.Shared.Contracts.ViewModels;
using DynamicPrice.Shared.Contracts.ViewModels.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DynamicPrice.Core.MediatR.ManagerEntity.Queries;

public class GetManagerInfoQueryHandler(
	DynamicPriceCoreContext context,
	IMapper mapper,
	IUserService userService)
	: IRequestHandler<GetManagerInfoQuery, ManagerInfoViewModel>
{
	public async Task<ManagerInfoViewModel> Handle(
		GetManagerInfoQuery request,
		CancellationToken cancellationToken)
	{
		var manager = await userService.GetRequiredCurrentUserAsync();

		var company = await context.Companies
			.FirstOrDefaultAsync(c => c.Id == manager.CompanyId, cancellationToken);

		return new ManagerInfoViewModel
		{
			Id = manager.Id ?? string.Empty,
			Name = manager.UserName ?? string.Empty,
			Email = manager.Email ?? string.Empty,
			Company = mapper.Map<CompanyViewModel>(company)
		};
	}
}
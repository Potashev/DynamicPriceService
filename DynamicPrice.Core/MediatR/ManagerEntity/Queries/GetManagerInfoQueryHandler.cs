using AutoMapper;
using DynamicPrice.Core.Data;
using DynamicPrice.Core.Exceptions;
using DynamicPrice.Core.Services;
using DynamicPrice.Shared.Contracts.ViewModels;
using DynamicPrice.Shared.Contracts.ViewModels.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DynamicPrice.Core.MediatR.ManagerEntity.Queries;

public class GetManagerInfoQueryHandler
	: IRequestHandler<GetManagerInfoQuery, ManagerInfoViewModel>
{
	private readonly DynamicPriceCoreContext _context;
	private readonly IMapper _mapper;
	private readonly IUserService _userService;

	public GetManagerInfoQueryHandler(
		DynamicPriceCoreContext context,
		IMapper mapper,
		IUserService userService)
		=> (_context, _mapper, _userService) = (context, mapper, userService);

	public async Task<ManagerInfoViewModel> Handle(
		GetManagerInfoQuery request,
		CancellationToken cancellationToken)
	{
		var manager = await _userService.GetRequiredCurrentUserAsync();

		var company = await _context.Companies
			.FirstOrDefaultAsync(c => c.CompanyId == manager.CompanyId, cancellationToken);

		return new ManagerInfoViewModel
		{
			Id = manager.Id ?? string.Empty,
			Name = manager.UserName ?? string.Empty,
			Email = manager.Email ?? string.Empty,
			Company = _mapper.Map<CompanyViewModel>(company)
		};
	}
}
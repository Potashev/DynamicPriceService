using AutoMapper;
using DynamicPrice.Core.Data;
using DynamicPrice.Core.Services;
using DynamicPrice.Shared.Contracts.ViewModels;
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
			Name = manager.UserName,
			Email = manager.Email,
			Company = _mapper.Map<CompanyViewModel>(company)
		};
	}
}
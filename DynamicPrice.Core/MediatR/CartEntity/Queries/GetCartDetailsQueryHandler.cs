using AutoMapper;
using DynamicPrice.Core.Data;
using DynamicPrice.Core.Services;
using DynamicPrice.Shared.Contracts.ViewModels.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DynamicPrice.Core.MediatR.CartEntity.Queries;

public class GetCartDetailsQueryHandler
	: IRequestHandler<GetCartDetailsQuery, CartViewModel>
{
	private readonly DynamicPriceCoreContext _context;
	private readonly IMapper _mapper;
	private readonly IUserService _userService;

	public GetCartDetailsQueryHandler(
		DynamicPriceCoreContext context,
		IMapper mapper,
		IUserService userService)
		=> (_context, _mapper, _userService) = (context, mapper, userService);

	public async Task<CartViewModel> Handle(
		GetCartDetailsQuery request,
		CancellationToken cancellationToken)
	{
		var customer = await _userService.GetRequiredCurrentUserAsync();

		var cart = await _context.Carts
			.Include(c => c.Company)
			.Include(c => c.CartItems)
				.ThenInclude(ci => ci.Product)
			.FirstOrDefaultAsync(c => c.CustomerId == customer.Id
				&& c.Company.CompanyId == request.CompanyId, cancellationToken);

		var cartVm = cart is not null
			? _mapper.Map<CartViewModel>(cart)
			: new CartViewModel
			{
				Company = _mapper.Map<CompanyViewModel>(await _context.Companies
					.FirstOrDefaultAsync(c => c.CompanyId == request.CompanyId, cancellationToken)),
				CartItems = Array.Empty<CartItemViewModel>()
			};

		return cartVm;
	}
}

using AutoMapper;
using DynamicPriceCore.Data;
using DynamicPriceCore.Models;
using DynamicPriceCore.Services;
using DynamicPriceCore.ViewModels;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DynamicPriceCore.MediatR.CartEntity.Queries;

public class GetCartDetailsQueryHandler
	: IRequestHandler<GetCartDetailsQuery, CartViewModel>
{
	private readonly DynamicPriceCoreContext _context;
	private readonly IMapper _mapper;
	private readonly IUserService _userService;

	public GetCartDetailsQueryHandler(DynamicPriceCoreContext context, IMapper mapper, IUserService userService)
		=> (_context, _mapper, _userService) = (context, mapper, userService);

	public async Task<CartViewModel> Handle(GetCartDetailsQuery request, CancellationToken cancellationToken)
	{
		var customer = await _userService.GetCurrentUserAsync();

		var cart = await _context.Carts
			.Include(c => c.Company)
			.Include(c => c.CartItems)
				.ThenInclude(ci => ci.Product)
			.FirstOrDefaultAsync(c => c.CustomerId == customer.Id
				&& c.Company.CompanyId == request.CompanyId, cancellationToken);

		return _mapper.Map<CartViewModel>(cart);
	}
}

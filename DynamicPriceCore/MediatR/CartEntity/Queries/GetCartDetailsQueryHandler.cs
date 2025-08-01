using AutoMapper;
using DynamicPriceCore.Data;
using DynamicPriceCore.MediatR.ViewModels;
using DynamicPriceCore.Models;
using DynamicPriceCore.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DynamicPriceCore.MediatR.CartEntity.Queries;

public class GetCartDetailsQueryHandler
	: IRequestHandler<GetCartDetailsQuery, CartViewModel>
{
	private readonly DynamicPriceCoreContext _context;
	private readonly IMapper _mapper;
	private readonly ICurrentUserService _currentUserService;

	public GetCartDetailsQueryHandler(DynamicPriceCoreContext context, IMapper mapper, ICurrentUserService currentUserService)
		=> (_context, _mapper, _currentUserService) = (context, mapper, currentUserService);

	public async Task<CartViewModel> Handle(GetCartDetailsQuery request, CancellationToken cancellationToken)
	{
		var customer = await _currentUserService.GetCurrentUserAsync();

		var cart = await _context.Carts
			.Include(c => c.Company)
			.Include(c => c.CartItems)
				.ThenInclude(ci => ci.Product)
			.FirstOrDefaultAsync(c => c.Customer.Id == customer.Id
				&& c.Company.CompanyId == request.CompanyId, cancellationToken);

		return _mapper.Map<CartViewModel>(cart);
	}
}

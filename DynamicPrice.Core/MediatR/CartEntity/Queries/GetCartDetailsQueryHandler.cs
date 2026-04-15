using AutoMapper;
using DynamicPrice.Core.Data;
using DynamicPrice.Core.Services;
using DynamicPrice.Shared.Contracts.ViewModels;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DynamicPrice.Core.MediatR.CartEntity.Queries;

public class GetCartDetailsQueryHandler(
	DynamicPriceCoreContext context,
	IMapper mapper,
	IUserService userService)
	: IRequestHandler<GetCartDetailsQuery, CartViewModel>
{
	public async Task<CartViewModel> Handle(
		GetCartDetailsQuery request,
		CancellationToken cancellationToken)
	{
		var customer = await userService.GetRequiredCurrentUserAsync();

		var cart = await context.Carts
			.Include(c => c.Company)
			.Include(c => c.CartItems)
				.ThenInclude(ci => ci.Product)
			.FirstOrDefaultAsync(c => c.CustomerId == customer.Id
				&& c.CompanyId == request.CompanyId, cancellationToken);

		var cartVm = cart is not null
			? mapper.Map<CartViewModel>(cart)
			: new CartViewModel
			{
				Company = mapper.Map<CompanyViewModel>(await context.Companies
					.FirstOrDefaultAsync(c => c.CompanyId == request.CompanyId, cancellationToken)),
				CartItems = Array.Empty<CartItemViewModel>()
			};

		return cartVm;
	}
}

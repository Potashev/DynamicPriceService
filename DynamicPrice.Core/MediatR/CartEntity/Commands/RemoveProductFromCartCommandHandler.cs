using AutoMapper;
using DynamicPrice.Core.Data;
using DynamicPrice.Core.Exceptions;
using DynamicPrice.Core.Services;
using DynamicPrice.Shared.Contracts.ViewModels;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DynamicPrice.Core.MediatR.CartEntity.Commands;

public class RemoveProductFromCartCommandHandler(
	DynamicPriceCoreContext context,
	IMapper mapper,
	IUserService userService)
	: IRequestHandler<RemoveProductFromCartCommand, CartViewModel>
{
	public async Task<CartViewModel> Handle(
		RemoveProductFromCartCommand request,
		CancellationToken cancellationToken)
	{
		var customer = await userService.GetRequiredCurrentUserAsync();

		var cart = await context.Carts
			.Where(c => c.CustomerId == customer.Id
				&& c.CartItems.Any(ci => ci.ProductId == request.ProductId))
			.Include(c => c.CartItems)
			.FirstOrDefaultAsync(cancellationToken)
			?? throw new NotFoundException("Cart not found");

		cart.RemoveItem(request.ProductId);

		await context.SaveChangesAsync(cancellationToken);

		return mapper.Map<CartViewModel>(cart);
	}
}

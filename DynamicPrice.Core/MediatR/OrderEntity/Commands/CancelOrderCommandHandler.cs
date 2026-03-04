using DynamicPrice.Core.Data;
using DynamicPrice.Core.Exceptions;
using DynamicPrice.Core.Models;
using DynamicPrice.Core.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DynamicPrice.Core.MediatR.OrderEntity.Commands;

public class CancelOrderCommandHandler
	: IRequestHandler<CancelOrderCommand>
{
	private readonly DynamicPriceCoreContext _context;
	private readonly IUserService _userService;

	public CancelOrderCommandHandler(
		DynamicPriceCoreContext context,
		IUserService userService)
		=> (_context, _userService) = (context, userService);

	public async Task Handle(
		CancelOrderCommand request,
		CancellationToken cancellationToken)
	{
		var customer = await _userService.GetRequiredCurrentUserAsync();

		var order = await _context.Orders
			.Where(o => o.OrderId == request.OrderId && o.CustomerId == customer.Id)
			.Include(o => o.OrderItems)
				.ThenInclude(oi => oi.Product)
			.FirstOrDefaultAsync(cancellationToken)
			?? throw new NotFoundException("Order not found.");

		order.MarkAsCanceled();

		await _context.SaveChangesAsync(cancellationToken);
		return;
	}
}

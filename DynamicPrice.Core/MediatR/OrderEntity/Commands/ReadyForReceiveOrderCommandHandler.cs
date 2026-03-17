using DynamicPrice.Core.Data;
using DynamicPrice.Core.Exceptions;
using DynamicPrice.Core.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DynamicPrice.Core.MediatR.OrderEntity.Commands;

public class ReadyForReceiveOrderCommandHandler
	: IRequestHandler<ReadyForReceiveOrderCommand>
{
	private readonly DynamicPriceCoreContext _context;
	private readonly IUserService _userService;

	public ReadyForReceiveOrderCommandHandler(
		DynamicPriceCoreContext context,
		IUserService userService)
		=> (_context, _userService) = (context, userService);

	public async Task Handle(
		ReadyForReceiveOrderCommand request,
		CancellationToken cancellationToken)
	{
		var manager = await _userService.GetRequiredCurrentUserAsync();

		var order = await _context.Orders
			.Where(o => o.OrderId.ToString() == request.OrderId && o.CompanyId == manager.CompanyId)
			.FirstOrDefaultAsync(cancellationToken)
			?? throw new NotFoundException("Order not found.");

		order.MarkAsReady();

		await _context.SaveChangesAsync(cancellationToken);
		return;
	}
}


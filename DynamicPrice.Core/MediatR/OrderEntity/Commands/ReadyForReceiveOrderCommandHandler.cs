using DynamicPrice.Core.Data;
using DynamicPrice.Core.Models;
using DynamicPrice.Core.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DynamicPrice.Core.MediatR.OrderEntity.Commands;

public class ReadyForReceiveOrderCommandHandler
	: IRequestHandler<ReadyForReceiveOrderCommand>
{
	private readonly DynamicPriceCoreContext _context;
	private readonly IUserService _userService;

	public ReadyForReceiveOrderCommandHandler(DynamicPriceCoreContext context, IUserService userService)
		=> (_context, _userService) = (context, userService);

	public async Task Handle(ReadyForReceiveOrderCommand request, CancellationToken cancellationToken)
	{
		var manager = await _userService.GetCurrentUserAsync();

		var order = await _context.Orders
			.Where(o => o.OrderId.ToString() == request.OrderId && o.Company.CompanyId == manager.CompanyId)
			.FirstOrDefaultAsync(cancellationToken);

		if (order is null)
			throw new Exception("Order not found.");

		if (order.Status is not OrderStatus.Confirmed)
			throw new Exception("Only confirmed orders can be set to ready for receive.");

		order.ReceiveKey = GenerateReceiveKey();
		order.Status = OrderStatus.Ready;

		await _context.SaveChangesAsync(cancellationToken);

		return;
	}

	private int GenerateReceiveKey() => new Random().Next(100000, 1000000); //TODO: make unique
}


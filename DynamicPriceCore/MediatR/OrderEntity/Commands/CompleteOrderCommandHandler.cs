using DynamicPrice.Core.Rabbit;
using DynamicPriceCore.Data;
using DynamicPriceCore.Models;
using DynamicPriceCore.Services;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DynamicPriceCore.MediatR.OrderEntity.Commands;

public class CompleteOrderCommandHandler
	: IRequestHandler<CompleteOrderCommand, int>
{
	private readonly DynamicPriceCoreContext _context;
	private readonly IUserService _userService;
    //private readonly IIncreasePriceService _increasePriceService;
    private readonly IPublishEndpoint _publishEndpoint;

    public CompleteOrderCommandHandler(DynamicPriceCoreContext context, IUserService userService, IPublishEndpoint publishEndpoint /*, IIncreasePriceService increasePriceService*/)
		=> (_context, _userService, _publishEndpoint /*, _increasePriceService*/) = (context, userService, publishEndpoint /*, increasePriceService*/);

	public async Task<int> Handle(CompleteOrderCommand request, CancellationToken cancellationToken)
	{
		var manager = await _userService.GetCurrentUserAsync();

		var order = await _context.Orders
			.Where(o => o.OrderId.ToString() == request.OrderId && o.Company.CompanyId == manager.CompanyId)
			.Include(o => o.OrderItems)
				.ThenInclude(oi => oi.Product)
			.FirstOrDefaultAsync(cancellationToken);

		if (order is null)
			throw new Exception("Order not found.");

		if (order.Status is not OrderStatus.Ready)
			throw new Exception("Only ready for receive orders can be completed.");

		var customer = await _userService.GetUserByIdAsync(order.CustomerId)
			?? throw new Exception("Customer not found.");

		var orderTotalAmount = order.OrderItems
			.Sum(oi => oi.Quantity * oi.ProductPrice);

		if (customer.Balance < orderTotalAmount)
			throw new Exception("Top up the balance.");

		customer.Balance -= orderTotalAmount;

		foreach (var oi in order.OrderItems)
		{
			oi.Product.LastSellTime = order.OrderDate; // todo: make now?
		}

		order.Status = OrderStatus.Completed;
		order.ReceiveKey = 0;	//todo: think about nullable?

		await _context.SaveChangesAsync(cancellationToken);
		await _userService.UpdateUserAsync(customer);

        await _publishEndpoint.Publish(new PriceIncreaseEvent(order.OrderItems), cancellationToken);

        //await _publishEndpoint.Publish(new PriceIncreaseEvent(order.OrderId), cancellationToken);
        //await _increasePriceService.Increase(order.OrderItems);

        return order.OrderId;
	}
}

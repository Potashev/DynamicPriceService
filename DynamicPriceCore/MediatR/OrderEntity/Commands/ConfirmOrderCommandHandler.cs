using DynamicPriceCore.Data;
using DynamicPriceCore.Models;
using DynamicPriceCore.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DynamicPriceCore.MediatR.OrderEntity.Commands;

public class ConfirmOrderCommandHandler
	: IRequestHandler<ConfirmOrderCommand, int>
{
	private readonly DynamicPriceCoreContext _context;
	private readonly IIncreasePriceService _increasePriceService;
	private readonly IUserService _userService;

	public ConfirmOrderCommandHandler(DynamicPriceCoreContext context, IIncreasePriceService increasePriceService, IUserService userService)
		=> (_context, _increasePriceService, _userService) = (context, increasePriceService, userService);

	public async Task<int> Handle(ConfirmOrderCommand request, CancellationToken cancellationToken)
	{
		var customer = await _userService.GetCurrentUserAsync();

		var cart = await _context.Carts
			.Where(c => c.CartId == request.CartId && c.CustomerId == customer.Id)
			.Include(c => c.Company)
			.Include(c => c.CartItems)
				.ThenInclude(ci => ci.Product)
			.FirstOrDefaultAsync(cancellationToken);

		if (cart == null)
			throw new Exception("Cart not found.");

		// todo: add total amount to order model?
		var orderTotalAmount = cart.CartItems
			.Sum(ci => ci.Quantity * ci.Product.Price);

		//if (customer.Balance < orderTotalAmount)		// COMPLETE
		//	throw new Exception("Top up the balance!");

		//customer.Balance -= orderTotalAmount;

		var order = new Order
		{
			CustomerId = customer.Id,
			Company = cart.Company,
			Status = OrderStatus.Confirmed,
			OrderDate = DateTime.UtcNow,
			//ReceiveKey = GenerateReceiveKey(),	// READY
			OrderItems = new List<OrderItem>()
		};

		foreach (var ci in cart.CartItems)
		{
			var product = ci.Product;

			order.OrderItems.Add(new OrderItem
			{
				Order = order,
				Product = product,
				ProductPrice = product.Price,	// can be changed since ordertotalamount?
				Quantity = ci.Quantity
			});

			//product.LastSellTime = order.OrderDate;	// COMPLETE

			if (product.Quantity != null)
				product.Quantity -= ci.Quantity;
		}


		_context.Orders.Add(order);
		_context.Carts.Remove(cart);	//cartitems removes too?

		await _context.SaveChangesAsync(cancellationToken);
		//await _userService.UpdateCurrentUserAsync();				// COMPLETE
		//await _increasePriceService.Increase(order.OrderItems);	// COMPLETE

		return order.OrderId;
	}

	//private int GenerateReceiveKey() => new Random().Next(100000, 1000000);	// READY
}

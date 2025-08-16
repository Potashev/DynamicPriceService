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
	private readonly ICurrentUserService _currentUserService;

	public ConfirmOrderCommandHandler(DynamicPriceCoreContext context, IIncreasePriceService increasePriceService, ICurrentUserService currentUserService)
		=> (_context, _increasePriceService, _currentUserService) = (context, increasePriceService, currentUserService);

	public async Task<int> Handle(ConfirmOrderCommand request, CancellationToken cancellationToken)
	{
		var customer = await _currentUserService.GetCurrentUserAsync();

		var cart = await _context.Carts
			.Where(c => c.CartId == request.CartId && c.CustomerId == customer.Id)  //todo: check
			.Include(c => c.Company)
			.Include(c => c.CartItems)
				.ThenInclude(ci => ci.Product)
			.FirstOrDefaultAsync(cancellationToken);

		if (cart == null)
			throw new Exception("Cart not found.");

		var orderTotalAmount = cart.CartItems
			.Sum(ci => ci.Quantity * ci.Product.Price);

		if (customer.Balance < orderTotalAmount)
			throw new Exception("Top up the balance!");

		customer.Balance -= orderTotalAmount;

		var order = new Order
		{
			CustomerId = customer.Id,
			Company = cart.Company,
			Status = OrderStatus.Confirmed,
			OrderDate = DateTime.UtcNow,
			ReceiveKey = GenerateReceiveKey(),
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

			product.LastSellTime = order.OrderDate;

			if (product.Quantity != null)
				product.Quantity -= ci.Quantity;
		}


		_context.Orders.Add(order);
		_context.Carts.Remove(cart);

		await _context.SaveChangesAsync(cancellationToken);

		await _increasePriceService.Increase(order.OrderItems);

		return order.ReceiveKey;
	}

	private int GenerateReceiveKey() => new Random().Next(100000, 1000000);
}

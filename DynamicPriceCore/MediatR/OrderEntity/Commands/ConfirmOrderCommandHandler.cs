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
	private readonly IUserService _userService;

	public ConfirmOrderCommandHandler(DynamicPriceCoreContext context, IUserService userService)
		=> (_context, _userService) = (context, userService);

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

		var order = new Order
		{
			Number = GenerateOrderNumber(),
			CustomerId = customer.Id,
			Company = cart.Company,
			Status = OrderStatus.Confirmed,
			OrderDate = DateTime.UtcNow,
			OrderItems = [] //todo: check
		};

		foreach (var ci in cart.CartItems)
		{
			var product = ci.Product;

			order.OrderItems.Add(new OrderItem
			{
				Order = order,
				Product = product,
				ProductPrice = product.Price,
				Quantity = ci.Quantity
			});

			if (product.Quantity != null)
				product.Quantity -= ci.Quantity;
		}


		_context.Orders.Add(order);
		_context.Carts.Remove(cart);    //todo: cartitems removes too?

		await _context.SaveChangesAsync(cancellationToken);

		return order.OrderId;
	}

	// todo: add test for uniqueness
	// Example: "3C-48291"
	private static string GenerateOrderNumber()
	{
		var guidBytes = Guid.NewGuid().ToByteArray();

		int firstDigit = guidBytes[0] % 10;
		char letter = (char)('A' + (guidBytes[1] % 26));
		int numberPart = BitConverter.ToInt32(guidBytes, 2) & 0x7FFFFFFF;
		string lastDigits = (numberPart % 100000).ToString("D5");

		return $"{firstDigit}{letter}-{lastDigits}";
	}
}

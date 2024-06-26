using AutoMapper;
using Azure.Core;
using DynamicPriceCore.Data;
using DynamicPriceCore.MediatR.ProductEntity.Commands;
using DynamicPriceCore.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DynamicPriceCore.MediatR.OrderEntity.Commands;

public class AddProductToOrderCommandHadnler
	: IRequestHandler<AddProductToOrderCommand, Order>
{
	private readonly DynamicPriceCoreContext _context;
	private readonly IMapper _mapper;

	public AddProductToOrderCommandHadnler(DynamicPriceCoreContext context, IMapper mapper)
		=> (_context, _mapper) = (context, mapper);

	public async Task<Order> Handle(AddProductToOrderCommand request, CancellationToken cancellationToken)
	{
		var product = await _context.Products
			.Include(p => p.Company)
			.Where(p => p.ProductId.ToString() == request.ProductId)
			.FirstOrDefaultAsync();

		var cartOrder = await _context.Orders
			.Include(o => o.Products)
			.Where(o => o.Customer.CustomerId.ToString() == request.CustomerId
				&& o.Company == product.Company
				&& o.Status == OrderStatus.Cart)
			.FirstOrDefaultAsync();

		if (cartOrder == null)
			cartOrder = await CreateNewOrder(request, product.Company);

		cartOrder.Products.Add(product);
		_context.SaveChanges();
		return cartOrder;
	}

	private async Task<Order> CreateNewOrder(AddProductToOrderCommand request, Company company)
	{
		var customer = await _context.Customers
			.Where(c => c.CustomerId.ToString() == request.CustomerId)
			.FirstOrDefaultAsync();

		var order = new Order
		{
			Customer = customer,
			Company = company,
			Status = OrderStatus.Cart,
			Products = new List<Product>()	//is it right?
		};

		await _context.Orders.AddAsync(order);
		await _context.SaveChangesAsync();
		return order;
	}
}

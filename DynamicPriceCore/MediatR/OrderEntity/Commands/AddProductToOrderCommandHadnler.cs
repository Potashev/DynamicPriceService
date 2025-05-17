using AutoMapper;
using DynamicPriceCore.Data;
using DynamicPriceCore.Models;
using DynamicPriceCore.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DynamicPriceCore.MediatR.OrderEntity.Commands;

public class AddProductToOrderCommandHadnler
	: IRequestHandler<AddProductToOrderCommand, Order>
{
	private readonly DynamicPriceCoreContext _context;
	private readonly IMapper _mapper;
	private readonly ICurrentUserService _currentUserService;

	public AddProductToOrderCommandHadnler(DynamicPriceCoreContext context, IMapper mapper, ICurrentUserService currentUserService)
		=> (_context, _mapper, _currentUserService) = (context, mapper, currentUserService);

	public async Task<Order> Handle(AddProductToOrderCommand request, CancellationToken cancellationToken)
	{
		//var customer = await _currentUserService.GetCurrentCustomerAsync();
		var customer = await _currentUserService.GetCurrentUserAsync();

		var product = await _context.Products
			.Include(p => p.Company)
			.Where(p => p.ProductId.ToString() == request.ProductId)
			.FirstOrDefaultAsync(cancellationToken);

		var cartOrder = await _context.Orders
			.Include(o => o.OrderProducts)
			.Where(o => o.Customer.Id == customer.Id
				&& o.Company == product.Company
				&& o.Status == OrderStatus.Cart)
			.FirstOrDefaultAsync(cancellationToken);

		if (cartOrder == null)
			cartOrder = await CreateNewOrder(customer, product.Company);

		var orderproduct = cartOrder.OrderProducts
			.Where(op => op.ProductId == product.ProductId)
			.FirstOrDefault();

		if (orderproduct == null)
		{
			//todo: make better
			orderproduct = new OrderProduct
			{
				Order = cartOrder,
				Product = product,
				Quantity = 1
			};
			cartOrder.OrderProducts.Add(orderproduct);
		}
		else
		{
			orderproduct.Quantity += 1;
		}
		await _context.SaveChangesAsync(cancellationToken);
		return cartOrder;
	}

	private async Task<Order> CreateNewOrder(ApplicationUser customer, Company company)
	{
		var order = new Order
		{
			Customer = customer,
			Company = company,
			Status = OrderStatus.Cart,
			OrderProducts = new List<OrderProduct>()	//is it right?
		};

		await _context.Orders.AddAsync(order);
		await _context.SaveChangesAsync();
		return order;
	}
}

using AutoMapper;
using DynamicPriceCore.Data;
using DynamicPriceCore.Models;
using DynamicPriceCore.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DynamicPriceCore.MediatR.OrderEntity.Commands;

public class RemoveProductFromOrderCommandHandler
	: IRequestHandler<RemoveProductFromOrderCommand, Order>
{
	private readonly DynamicPriceCoreContext _context;
	private readonly IMapper _mapper;
	private readonly ICurrentUserService _currentUserService;

	public RemoveProductFromOrderCommandHandler(DynamicPriceCoreContext context, IMapper mapper, ICurrentUserService currentUserService)
		=> (_context, _mapper, _currentUserService) = (context, mapper, currentUserService);

	public async Task<Order> Handle(RemoveProductFromOrderCommand request, CancellationToken cancellationToken)
	{
		var customer = await _currentUserService.GetCurrentCustomerAsync();

		var orderproduct = await _context.OrderProducts
			.Where(op => op.ProductId.ToString() == request.ProductId
				&& op.Order.Customer.Id == customer.Id	//todo: check
				&& op.Order.Status == OrderStatus.Cart)
			.Include(op => op.Order)
				.ThenInclude(o => o.Company)
			.FirstOrDefaultAsync(cancellationToken);

		orderproduct.Quantity -= 1;

		if (orderproduct.Quantity == 0)
			_context.OrderProducts.Remove(orderproduct);

		await _context.SaveChangesAsync(cancellationToken);

		return orderproduct.Order;
	}
}

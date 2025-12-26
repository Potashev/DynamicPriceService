using DynamicPrice.Core.Data;
using DynamicPrice.Core.Exceptions;
using DynamicPrice.Core.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DynamicPrice.Core.MediatR.OrderEntity.Queries;

public class GetOrderIdByReceiveKeyQueryHandler
	: IRequestHandler<GetOrderIdByReceiveKeyQuery, int>
{
	private readonly DynamicPriceCoreContext _context;
	private readonly IUserService _userService;

	public GetOrderIdByReceiveKeyQueryHandler(DynamicPriceCoreContext context, IUserService userService)
		=> (_context, _userService) = (context, userService);

	public async Task<int> Handle(GetOrderIdByReceiveKeyQuery request, CancellationToken cancellationToken)
	{
		var manager = await _userService.GetCurrentUserAsync();

		var orderId = await _context.Orders
			.Where(o => o.ReceiveKey.ToString() == request.ReceiveKey && o.Company.CompanyId == manager.CompanyId)
			.Select(o => o.OrderId)
			.FirstOrDefaultAsync(cancellationToken);

		return orderId == 0
			? throw new NotFoundException($"Order with receive key '{request.ReceiveKey}' not found.")
			: orderId;
	}
}

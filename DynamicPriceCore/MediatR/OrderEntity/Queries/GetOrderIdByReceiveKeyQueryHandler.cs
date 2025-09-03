using AutoMapper;
using DynamicPriceCore.Data;
using DynamicPriceCore.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DynamicPriceCore.MediatR.OrderEntity.Queries;

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
			//todo: check and add additional filter or make uniq key
			.Where(o => o.ReceiveKey.ToString() == request.ReceiveKey && o.Company.CompanyId == manager.CompanyId)
			.Select(o => o.OrderId)
			.FirstOrDefaultAsync(cancellationToken);

		// todo: handle null case

		return orderId;
	}
}

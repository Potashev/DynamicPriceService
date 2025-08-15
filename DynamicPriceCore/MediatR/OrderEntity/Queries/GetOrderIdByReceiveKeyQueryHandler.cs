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
	private readonly ICurrentUserService _currentUserService;

	public GetOrderIdByReceiveKeyQueryHandler(DynamicPriceCoreContext context, ICurrentUserService currentUserService)
		=> (_context, _currentUserService) = (context, currentUserService);

	public async Task<int> Handle(GetOrderIdByReceiveKeyQuery request, CancellationToken cancellationToken)
	{
		var manager = await _currentUserService.GetCurrentUserAsync();

		var orderId = await _context.Orders
			//todo: check and add additional filter or make uniq key
			.Where(o => o.ReceiveKey.ToString() == request.ReceiveKey && o.Company.CompanyId == manager.CompanyId)
			.Select(o => o.OrderId)
			.FirstOrDefaultAsync(cancellationToken);

		// todo: handle null case

		return orderId;
	}
}

using AutoMapper;
using DynamicPriceCore.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DynamicPriceCore.MediatR.OrderEntity.Queries;

public class GetOrderIdByReceiveKeyQueryHandler
	: IRequestHandler<GetOrderIdByReceiveKeyQuery, int>
{
	private readonly DynamicPriceCoreContext _context;
	private readonly IMapper _mapper;

	public GetOrderIdByReceiveKeyQueryHandler(DynamicPriceCoreContext context, IMapper mapper)
		=> (_context, _mapper) = (context, mapper);

	public async Task<int> Handle(GetOrderIdByReceiveKeyQuery request, CancellationToken cancellationToken)
	{
		var orderId = await _context.Orders
			.Where(o => o.ReceiveKey.ToString() == request.ReceiveKey)  //todo: add additional filter or make uniq key
			.Select(o => o.OrderId)
			.FirstOrDefaultAsync(cancellationToken);

		// todo: handle null case

		return orderId;
	}
}

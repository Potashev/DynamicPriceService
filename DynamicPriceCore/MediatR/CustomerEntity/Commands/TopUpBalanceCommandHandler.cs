using AutoMapper;
using DynamicPriceCore.Data;
using DynamicPriceCore.MediatR.CustomerEntity.Commands;
using DynamicPriceCore.Models;
using DynamicPriceCore.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace DynamicPriceCore.MediatR.CustomerEntity.Commands;

public class TopUpBalanceCommandHandler
	: IRequestHandler<TopUpBalanceCommand>
{
	private readonly DynamicPriceCoreContext _context;
	private readonly ICurrentUserService _currentUserService;

	public TopUpBalanceCommandHandler(DynamicPriceCoreContext context, IIncreasePriceService increasePriceService, ICurrentUserService currentUserService)
		=> (_context, _currentUserService) = (context, currentUserService);

	public async Task Handle(TopUpBalanceCommand request, CancellationToken cancellationToken)
	{
		var customer = await _currentUserService.GetCurrentUserAsync();

		//todo: check
		if (customer == null) { }

		var replenishmentAmount = decimal.Parse(request.balanceVm.ReplenishmentAmount, CultureInfo.InvariantCulture); ;

		customer.Balance += replenishmentAmount;

		await _context.SaveChangesAsync(cancellationToken);
	}
}

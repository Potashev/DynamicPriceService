using AutoMapper;
using DynamicPriceCore.Data;
using DynamicPriceCore.MediatR.CustomerEntity.Commands;
using DynamicPriceCore.Models;
using DynamicPriceCore.Services;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace DynamicPriceCore.MediatR.CustomerEntity.Commands;

public class TopUpBalanceCommandHandler
	: IRequestHandler<TopUpBalanceCommand>
{
	private readonly IUserService _userService;

	public TopUpBalanceCommandHandler(IUserService userService)
		=> _userService = userService;

	public async Task Handle(TopUpBalanceCommand request, CancellationToken cancellationToken)
	{
		var customer = await _userService.GetCurrentUserAsync();

		if (customer == null) { }

		var replenishmentAmount = decimal.Parse(request.balanceVm.ReplenishmentAmount, CultureInfo.InvariantCulture);

		customer.Balance += replenishmentAmount;

		// can user be changed since last GetCurrentUserAsync?
		await _userService.UpdateCurrentUserAsync();
	}
}

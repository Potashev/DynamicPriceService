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
	private readonly UserManager<ApplicationUser> _userManager;
	private readonly ICurrentUserService _currentUserService;

	public TopUpBalanceCommandHandler(DynamicPriceCoreContext context, UserManager<ApplicationUser> userManager, ICurrentUserService currentUserService)
		=> (_userManager, _currentUserService) = (userManager, currentUserService);

	public async Task Handle(TopUpBalanceCommand request, CancellationToken cancellationToken)
	{
		//var customer = await _currentUserService.GetCurrentUserAsync();
		var customer = await _userManager.FindByIdAsync(_currentUserService.UserId);

		if (customer == null) { }

		var replenishmentAmount = decimal.Parse(request.balanceVm.ReplenishmentAmount, CultureInfo.InvariantCulture);

		customer.Balance += replenishmentAmount;


		await _userManager.UpdateAsync(customer);
	}
}

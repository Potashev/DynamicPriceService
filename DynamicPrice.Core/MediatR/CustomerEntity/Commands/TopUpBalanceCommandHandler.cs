using DynamicPrice.Core.Services;
using MediatR;
using System.Globalization;

namespace DynamicPrice.Core.MediatR.CustomerEntity.Commands;

public class TopUpBalanceCommandHandler
	: IRequestHandler<TopUpBalanceCommand>
{
	private readonly IUserService _userService;

	public TopUpBalanceCommandHandler(IUserService userService)
		=> _userService = userService;

	public async Task Handle(
		TopUpBalanceCommand request,
		CancellationToken cancellationToken)
	{
		var customer = await _userService.GetRequiredCurrentUserAsync();

		var replenishmentAmount = decimal.Parse(request.balanceVm.ReplenishmentAmount, CultureInfo.InvariantCulture);

		customer.Balance += replenishmentAmount;

		await _userService.UpdateCurrentUserAsync();
	}
}

using DynamicPrice.Core.Services;
using MediatR;

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

		customer.Balance += request.balanceVm.ReplenishmentAmount;

		await _userService.UpdateCurrentUserAsync();
	}
}

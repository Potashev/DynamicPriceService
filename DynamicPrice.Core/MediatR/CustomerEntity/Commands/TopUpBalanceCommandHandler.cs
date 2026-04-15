using DynamicPrice.Core.Exceptions;
using DynamicPrice.Core.Services;
using MediatR;

namespace DynamicPrice.Core.MediatR.CustomerEntity.Commands;

public class TopUpBalanceCommandHandler(
	IUserService userService)
	: IRequestHandler<TopUpBalanceCommand>
{
	public async Task Handle(
		TopUpBalanceCommand request,
		CancellationToken cancellationToken)
	{
		var customer = await userService.GetRequiredCurrentUserAsync();

		customer.Balance += request.balanceVm.ReplenishmentAmount;

		await userService.UpdateCurrentUserAsync();
	}
}

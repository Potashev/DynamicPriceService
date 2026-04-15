using DynamicPrice.Core.Exceptions;
using DynamicPrice.Core.Services;
using DynamicPrice.Shared.Contracts.Requests;
using MediatR;

namespace DynamicPrice.Core.MediatR.ManagerEntity.Commands;

public class RegisterManagerCommandHandler(
	IUserService userService)
	: IRequestHandler<RegisterManagerCommand>
{
	public async Task Handle(
	RegisterManagerCommand request,
	CancellationToken cancellationToken)
	{
		var manager = await userService.GetRequiredCurrentUserAsync();

		await userService.RegisterUserAsync(
			new RegisterUserRequest(request.registerVm)
			{
				Role = "Manager",
				CompanyId = manager.CompanyId
			});
	}
}

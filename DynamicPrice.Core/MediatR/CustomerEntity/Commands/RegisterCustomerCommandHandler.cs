using DynamicPrice.Core.Exceptions;
using DynamicPrice.Core.Services;
using DynamicPrice.Shared.Contracts.Requests;
using MediatR;

namespace DynamicPrice.Core.MediatR.CustomerEntity.Commands;

public class RegisterCustomerCommandHandler(
	IUserService userService)
	: IRequestHandler<RegisterCustomerCommand>
{
	public async Task Handle(
	RegisterCustomerCommand request,
	CancellationToken cancellationToken)
		=> await userService.RegisterUserAsync(
			new RegisterUserRequest(request.registerVm)
			{
				Role = "Customer",
				Balance = 0
			});
}

using AutoMapper;
using DynamicPrice.Core.Services;
using DynamicPrice.Shared.Contracts.Requests;
using MediatR;

namespace DynamicPrice.Core.MediatR.CustomerEntity.Commands;

public class RegisterCustomerCommandHandler
	: IRequestHandler<RegisterCustomerCommand>
{
	private readonly IUserService _userService;

	public RegisterCustomerCommandHandler(IUserService userService)
		=> _userService = userService;

	public async Task Handle(
	RegisterCustomerCommand request,
	CancellationToken cancellationToken)
		=> await _userService.RegisterUserAsync(
			new RegisterUserRequest(request.registerVm) 
			{
				Role = "Customer",
				Balance = 0
			});
}

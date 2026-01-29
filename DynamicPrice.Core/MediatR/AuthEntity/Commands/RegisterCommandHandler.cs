using DynamicPrice.Core.Services;
using MediatR;

namespace DynamicPrice.Core.MediatR.AuthEntity.Commands;

public class RegisterCommandHandler
	: IRequestHandler<RegisterCommand>
{
	private readonly IUserService _userService;

	// todo: di
	public RegisterCommandHandler(IUserService userService)
		=> _userService = userService;

	public async Task Handle(
		RegisterCommand request,
		CancellationToken cancellationToken)
		=> await _userService.RegisterUserAsync(
			request.registerVm.Username,
			request.registerVm.Password,
			request.registerVm.Email,
			request.registerVm.Role);
}

using DynamicPrice.Core.Services;
using MediatR;

namespace DynamicPrice.Core.MediatR.AuthEntity.Commands;

public class LoginCommandHandler
	: IRequestHandler<LoginCommand, string>
{
	private readonly IUserService _userService;

	public LoginCommandHandler(IUserService userService)
		=> _userService = userService;

	public async Task<string> Handle(
		LoginCommand request,
		CancellationToken cancellationToken)
		=> await _userService.LoginUserAsync(request.loginVm.Username, request.loginVm.Password);
}

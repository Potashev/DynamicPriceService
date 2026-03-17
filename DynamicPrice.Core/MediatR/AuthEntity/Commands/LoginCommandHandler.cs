using DynamicPrice.Core.Services;
using DynamicPrice.Shared.Contracts.Responses;
using MediatR;

namespace DynamicPrice.Core.MediatR.AuthEntity.Commands;

public class LoginCommandHandler
	: IRequestHandler<LoginCommand, TokenResponse>
{
	private readonly IUserService _userService;

	public LoginCommandHandler(IUserService userService)
		=> _userService = userService;

	public async Task<TokenResponse> Handle(
		LoginCommand request,
		CancellationToken cancellationToken)
		=> await _userService.LoginUserAsync(request.loginVm);
}

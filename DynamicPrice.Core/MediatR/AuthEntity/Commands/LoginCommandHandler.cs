using DynamicPrice.Core.Services;
using DynamicPrice.Shared.Contracts.Responses;
using MediatR;

namespace DynamicPrice.Core.MediatR.AuthEntity.Commands;

public class LoginCommandHandler(
	IUserService userService)
	: IRequestHandler<LoginCommand, TokenResponse>
{
	public async Task<TokenResponse> Handle(
		LoginCommand request,
		CancellationToken cancellationToken)
		=> await userService.LoginUserAsync(request.loginVm);
}

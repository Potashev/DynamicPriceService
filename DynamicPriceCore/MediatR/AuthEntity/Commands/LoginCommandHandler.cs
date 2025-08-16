using DynamicPriceCore.Data;
using DynamicPriceCore.Models;
using DynamicPriceCore.Services;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DynamicPriceCore.MediatR.AuthEntity.Commands;

public class LoginCommandHandler
	: IRequestHandler<LoginCommand, string>
{
	private readonly ICurrentUserService _currentUserService;

	public LoginCommandHandler(ICurrentUserService currentUserService)
		=> _currentUserService = currentUserService;

	public async Task<string> Handle(LoginCommand request, CancellationToken cancellationToken)
	{
		var userName = request.loginVm.Username;
		var password = request.loginVm.Password;

		var token = await _currentUserService.LoginUserAsync(userName, password);
		return token;
	}
}

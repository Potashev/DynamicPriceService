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
	private readonly IUserService _userService;

	public LoginCommandHandler(IUserService userService)
		=> _userService = userService;

	public async Task<string> Handle(LoginCommand request, CancellationToken cancellationToken)
		=> await _userService.LoginUserAsync(request.loginVm.Username, request.loginVm.Password);
}

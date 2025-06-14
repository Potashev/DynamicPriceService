using DynamicPriceCore.Data;
using DynamicPriceCore.Models;
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
	private readonly UserManager<ApplicationUser> _userManager;
	private readonly IConfiguration _config;

	public LoginCommandHandler(UserManager<ApplicationUser> userManager, IConfiguration config)
		=> (_userManager, _config) = (userManager, config);

	public async Task<string> Handle(LoginCommand request, CancellationToken cancellationToken)
	{
		var user = await _userManager.FindByNameAsync(request.loginVm.Username);
		if (user == null || !await _userManager.CheckPasswordAsync(user, request.loginVm.Password))
			throw new ArgumentException("Unauthorized!");

		var roles = await _userManager.GetRolesAsync(user);
		var token = GenerateJwtToken(user, roles);
		return token;

	}

	private string GenerateJwtToken(ApplicationUser user, IList<string> roles)
	{
		var claims = new List<Claim>
		{
			new Claim(JwtRegisteredClaimNames.Sub, user.Id),
			new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName)
		};

		claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

		var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
		var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
		var token = new JwtSecurityToken(
			issuer: _config["Jwt:Issuer"],
			audience: _config["Jwt:Audience"],
			claims: claims,
			expires: DateTime.UtcNow.AddHours(1),
			signingCredentials: creds);

		return new JwtSecurityTokenHandler().WriteToken(token);
	}
}

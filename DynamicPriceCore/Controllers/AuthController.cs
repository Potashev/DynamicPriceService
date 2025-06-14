using DynamicPriceCore.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System;
using DynamicPriceCore.Data;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using DynamicPriceCore.MediatR.ViewModels;

namespace DynamicPriceCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly IConfiguration _config;
		private readonly DynamicPriceCoreContext _context;

		public AuthController(UserManager<ApplicationUser> userManager, IConfiguration config, DynamicPriceCoreContext context)
		{
			_userManager = userManager;
			_config = config;
			_context = context;
		}
		

		[HttpPost("/api/Register")]
		public async Task<IActionResult> Register([FromBody] RegisterViewModel registerVm)
		{
			//todo: test - make better
			ApplicationUser user;
			if (registerVm.Role == "Customer")
			{
				user = new ApplicationUser { UserName = registerVm.Username, Email = registerVm.Email, Balance = 0 };
			}
			else if (registerVm.Role == "Manager")
			{
				user = new ApplicationUser
				{
					UserName = registerVm.Username, 
					Email = registerVm.Email,
					CompanyId = 1	//todo: fixed
				};
				
			}
			else
			{
				return BadRequest("Invalid role specified.");
			}

			//for testing
			try
			{
				var result = await _userManager.CreateAsync(user, registerVm.Password);
				if (!result.Succeeded) return BadRequest(result.Errors);

				await _userManager.AddToRoleAsync(user, registerVm.Role);
				await _context.SaveChangesAsync();
			}
			catch (Exception ex) 
			{
			
			}

			return Ok("User registered successfully");
		}

		[HttpPost("/api/Login")]
		public async Task<IActionResult> Login([FromBody] LoginViewModel model)
		{
			var user = await _userManager.FindByNameAsync(model.Username);
			if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
				return Unauthorized();

			//for testing
			try
			{
				var roles = await _userManager.GetRolesAsync(user);
				var token = GenerateJwtToken(user, roles);
				return Ok(new { token });
			}
			catch (Exception ex)
			{

			}

			return BadRequest();
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
}

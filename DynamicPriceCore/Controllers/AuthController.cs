using DynamicPriceCore.Data;
using DynamicPriceCore.MediatR.AuthEntity.Commands;
using DynamicPriceCore.MediatR.CompanyEntity.Queries;
using DynamicPriceCore.MediatR.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DynamicPriceCore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
		private readonly IMediator _mediator;

		public AuthController(IMediator mediator)
		{
			_mediator = mediator;
		}
		

		[HttpPost("/api/Register")]
		public async Task<IActionResult> Register([FromBody] RegisterViewModel registerVm)
		{
			await _mediator.Send(new RegisterCommand(registerVm));
			return Ok("User registered successfully");
		}

		[HttpPost("/api/Login")]
		public async Task<IActionResult> Login([FromBody] LoginViewModel loginVm)
		{
			var token = await _mediator.Send(new LoginCommand(loginVm));
			return Ok(new { token });
		}
	}
}

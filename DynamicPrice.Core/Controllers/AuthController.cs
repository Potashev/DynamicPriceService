//using DynamicPrice.Core.MediatR.AuthEntity.Commands;
//using DynamicPrice.Shared.Contracts.ViewModels;
//using MediatR;
//using Microsoft.AspNetCore.Mvc;

//namespace DynamicPrice.Core.Controllers;

//[ApiController]
//[Route("api/[controller]")]
//public class AuthController : ControllerBase
//{
//	private readonly IMediator _mediator;

//	public AuthController(IMediator mediator)
//	{
//		_mediator = mediator;
//	}

//	[HttpPost("register")]
//	public async Task<IActionResult> Register([FromBody] RegisterViewModel registerVm)
//	{
//		await _mediator.Send(new RegisterCommand(registerVm));
//		return Ok("User registered successfully");
//	}

//	[HttpPost("login")]
//	public async Task<IActionResult> Login([FromBody] LoginViewModel loginVm)
//	{
//		var token = await _mediator.Send(new LoginCommand(loginVm));
//		return Ok(new { token });
//	}
//}

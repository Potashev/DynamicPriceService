using DynamicPrice.Core.MediatR.AuthEntity.Commands;
using DynamicPrice.Shared.Contracts.ViewModels;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DynamicPrice.Core.MinimalAPI;

public static class AuthEndPoints
{
	public static void MapAuthEndPoints(this IEndpointRouteBuilder app)
	{
		var priceRule = app.MapGroup("/api/auth");

		priceRule.MapPost("/register", Register)
			.WithSummary("Creates a product");
		priceRule.MapPost("/login", Login)
			.WithSummary("Creates a product");
	}

	//[HttpPost("register")]
	private static async Task<IResult> Register([FromBody] RegisterViewModel registerVm, IMediator mediator)
	{
		await mediator.Send(new RegisterCommand(registerVm));
		return Results.Ok("User registered successfully");
	}

	//[HttpPost("login")]
	private static async Task<IResult> Login([FromBody] LoginViewModel loginVm, IMediator mediator)
	{
		var token = await mediator.Send(new LoginCommand(loginVm));
		return Results.Ok(new { token });
	}
}

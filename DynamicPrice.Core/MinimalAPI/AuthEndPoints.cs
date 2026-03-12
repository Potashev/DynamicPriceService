using DynamicPrice.Core.MediatR.AuthEntity.Commands;
using DynamicPrice.Shared.Contracts.Requests;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DynamicPrice.Core.MinimalAPI;

public static class AuthEndPoints
{
	public static void MapAuthEndPoints(this IEndpointRouteBuilder app)
	{
		var priceRule = app.MapGroup("/api/auth");

		priceRule.MapPost("/register", Register)
			.WithSummary("Регистрация пользователя");
		priceRule.MapPost("/login", Login)
			.WithSummary("Аутентификация пользователя");

		//todo: fixed url
		priceRule.MapPost("/register/manager", RegisterManager)
			.WithSummary("Регистрация пользователя")
			.RequireAuthorization("ManagerPolicy");
	}

	private static async Task<IResult> Register(
		[FromBody] RegisterRequest registerVm,
		IMediator mediator)
	{
		await mediator.Send(new RegisterCustomerCommand(registerVm));
		return Results.Ok("User registered successfully");
	}

	private static async Task<IResult> RegisterManager(
	[FromBody] RegisterRequest registerVm,
	IMediator mediator)
	{
		await mediator.Send(new RegisterManagerCommand(registerVm));
		return Results.Ok("User registered successfully");
	}

	private static async Task<IResult> Login(
		[FromBody] LoginRequest loginVm,
		IMediator mediator)
	{
		var token = await mediator.Send(new LoginCommand(loginVm));
		return Results.Ok(new { token });
	}
}

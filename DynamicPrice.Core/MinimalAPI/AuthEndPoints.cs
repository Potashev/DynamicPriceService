using DynamicPrice.Core.MediatR.AuthEntity.Commands;
using DynamicPrice.Shared.Contracts.Requests;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DynamicPrice.Core.MinimalAPI;

public static class AuthEndPoints
{
	public static void MapAuthEndPoints(this IEndpointRouteBuilder app)
	{
		app.MapPost("/api/auth/login", Login)
			.WithSummary("Аутентификация пользователя");
	}

	private static async Task<IResult> Login(
		[FromBody] LoginRequest loginVm,
		IMediator mediator,
		CancellationToken cancellationToken)
			=> Results.Ok(await mediator.Send(new LoginCommand(loginVm), cancellationToken));
}

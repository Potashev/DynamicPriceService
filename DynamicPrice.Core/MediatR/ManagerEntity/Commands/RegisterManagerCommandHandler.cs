using AutoMapper;
using DynamicPrice.Core.Services;
using DynamicPrice.Shared.Contracts.Requests;
using MediatR;

namespace DynamicPrice.Core.MediatR.ManagerEntity.Commands;

public class RegisterManagerCommandHandler
	: IRequestHandler<RegisterManagerCommand>
{
	private readonly IUserService _userService;

	public RegisterManagerCommandHandler(IUserService userService)
		=> _userService = userService;

	public async Task Handle(
	RegisterManagerCommand request,
	CancellationToken cancellationToken)
	{
		var manager = await _userService.GetRequiredCurrentUserAsync();

		await _userService.RegisterUserAsync(
			new RegisterUserRequest(request.registerVm)
			{
				Role = "Manager",
				CompanyId = manager.CompanyId
			});
	}
}

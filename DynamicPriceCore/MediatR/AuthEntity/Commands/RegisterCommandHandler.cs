using DynamicPriceCore.Services;
using MediatR;

namespace DynamicPriceCore.MediatR.AuthEntity.Commands;

public class RegisterCommandHandler
	: IRequestHandler<RegisterCommand>
{
	private readonly ICurrentUserService _currentUserService;

	public RegisterCommandHandler(ICurrentUserService currentUserService)
		=> _currentUserService = currentUserService;

	public async Task Handle(RegisterCommand request, CancellationToken cancellationToken)
		=> await _currentUserService.RegisterUserAsync(
			request.registerVm.Username,
			request.registerVm.Password,
			request.registerVm.Email,
			request.registerVm.Role);
}

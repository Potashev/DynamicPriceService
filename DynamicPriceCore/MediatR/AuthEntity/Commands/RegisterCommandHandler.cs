using DynamicPriceCore.Data;
using DynamicPriceCore.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace DynamicPriceCore.MediatR.AuthEntity.Commands;

public class RegisterCommandHandler
	: IRequestHandler<RegisterCommand>
{
	private readonly DynamicPriceCoreContext _context;
	private readonly UserManager<ApplicationUser> _userManager;

	public RegisterCommandHandler(DynamicPriceCoreContext context, UserManager<ApplicationUser> userManager)
		=> (_context, _userManager) = (context, userManager);

	public async Task Handle(RegisterCommand request, CancellationToken cancellationToken)
	{
		//todo: check and maybe make better
		ApplicationUser user = new ApplicationUser
		{
			UserName = request.registerVm.Username,
			Email = request.registerVm.Email
		};

		switch (request.registerVm.Role)
		{
			case "Customer":
				user.Balance = 0;
				break;

			case "Manager":
				user.CompanyId = 1; //todo: replace with actual
				break;

			default:
				throw new ArgumentException("Invalid user role");
		}

		var result = await _userManager.CreateAsync(user, request.registerVm.Password);
		if (!result.Succeeded)
		{
			throw new ApplicationException($"User creation failed!");
		}

		await _userManager.AddToRoleAsync(user, request.registerVm.Role);

		await _context.SaveChangesAsync(cancellationToken);
	}
}

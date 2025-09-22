using DynamicPrice.Core.Rabbit;
using DynamicPriceCore.Data;
using DynamicPriceCore.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace DynamicPriceCore.MediatR.PriceRuleEntity.Commands;

public class PriceReducingCommandHandler
	: IRequestHandler<PriceReducingCommand>
{
	private readonly IUserService _userService;
	private readonly IEventBus _eventBus;

	public PriceReducingCommandHandler(IUserService userService, IEventBus eventBus)
		=> (_userService, _eventBus) = (userService, eventBus);

	public async Task Handle(PriceReducingCommand request, CancellationToken cancellationToken)
	{
		var manager = await _userService.GetCurrentUserAsync();

		var companyId = (int)manager.CompanyId;

		if (request.IsRunCommand)
			await _eventBus.PublishAsync(new CompanyMonitoringStarted(companyId), "company.start");
		else
			await _eventBus.PublishAsync(new CompanyMonitoringStopped(companyId), "company.stop");

		return;
	}
}

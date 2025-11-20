using DynamicPrice.Core.Rabbit;
using DynamicPriceCore.Services;
using MassTransit;
using MediatR;

namespace DynamicPriceCore.MediatR.PriceRuleEntity.Commands;

public class PriceReducingCommandHandler
	: IRequestHandler<PriceReducingCommand>
{
	private readonly IUserService _userService;
	//private readonly IEventBus _eventBus;
	private readonly IPublishEndpoint _publishEndpoint;

    public PriceReducingCommandHandler(IUserService userService, IPublishEndpoint publishEndpoint)
		=> (_userService, _publishEndpoint) = (userService, publishEndpoint);

	public async Task Handle(PriceReducingCommand request, CancellationToken cancellationToken)
	{
		var manager = await _userService.GetCurrentUserAsync();

		var companyId = (int)manager.CompanyId;

        //if (request.IsRunCommand)
        //	await _eventBus.PublishAsync(new CompanyMonitoringStarted(companyId), "company.start");
        //else
        //	await _eventBus.PublishAsync(new CompanyMonitoringStopped(companyId), "company.stop");

        //if (request.IsRunCommand)
        //    await _publishEndpoint.Publish(new CompanyMonitoringStarted(companyId), cancellationToken);
        //else
        //    await _publishEndpoint.Publish(new CompanyMonitoringStopped(companyId), cancellationToken);

        if (request.IsRunCommand)
            await _publishEndpoint.Publish(new CompanyMonitoringEvent(companyId, "start"), cancellationToken);
        else
            await _publishEndpoint.Publish(new CompanyMonitoringEvent(companyId, "stop"), cancellationToken);

        return;
	}
}

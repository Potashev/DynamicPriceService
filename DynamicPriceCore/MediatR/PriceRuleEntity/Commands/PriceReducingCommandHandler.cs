using DynamicPrice.Core.Services;
using DynamicPriceCore.Services;
using MassTransit;
using MediatR;

namespace DynamicPriceCore.MediatR.PriceRuleEntity.Commands;

public class PriceReducingCommandHandler
	: IRequestHandler<PriceReducingCommand>
{
	private readonly IUserService _userService;
	private readonly IPublishEndpoint _publishEndpoint;

    public PriceReducingCommandHandler(IUserService userService, IPublishEndpoint publishEndpoint)
		=> (_userService, _publishEndpoint) = (userService, publishEndpoint);

	public async Task Handle(PriceReducingCommand request, CancellationToken cancellationToken)
	{
		var manager = await _userService.GetCurrentUserAsync();

		var companyId = (int)manager.CompanyId;

        if (request.IsRunCommand)
            await _publishEndpoint.Publish(new CompanyMonitoringEvent(companyId, MonitoringEvent.Start), cancellationToken);
        else
            await _publishEndpoint.Publish(new CompanyMonitoringEvent(companyId, MonitoringEvent.Stop), cancellationToken);

        return;
	}
}

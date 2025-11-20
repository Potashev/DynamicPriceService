using DynamicPrice.Core.Rabbit;
using DynamicPriceCore.Data;
using DynamicPriceCore.Models;
using MassTransit;

namespace DynamicPriceCore.Services;

//todo: make as background service?
public class ActiveCompaniesService : IConsumer<CompanyMonitoringEvent>
{
	private readonly IServiceProvider _serviceProvider;

	public ActiveCompaniesService(IServiceProvider serviceProvider, IConfiguration config, IEventBus eventBus)
	{
		_serviceProvider = serviceProvider;
	}

    public async Task Consume(ConsumeContext<CompanyMonitoringEvent> consumeContext)
    {
		var companyId = consumeContext.Message.CompanyId;
		var monitoringEvent = consumeContext.Message.Event;

		using var scope = _serviceProvider.CreateScope();
		var context = scope.ServiceProvider.GetRequiredService<DynamicPriceCoreContext>();

		if (monitoringEvent == "start")
		{
			var entity = await context.ActiveCompanies.FindAsync(companyId);
			if (entity == null)
			{
                //todo: set lastmonitoring as default?
                context.ActiveCompanies.Add(new ActiveCompany { CompanyId = companyId, StartedAt = DateTime.UtcNow });
			}
		}
		else if (monitoringEvent == "stop")
		{
			var entity = await context.ActiveCompanies.FindAsync(companyId);
			if (entity != null)
			{
				context.ActiveCompanies.Remove(entity);
			}
		}

        await context.SaveChangesAsync();
    }
}

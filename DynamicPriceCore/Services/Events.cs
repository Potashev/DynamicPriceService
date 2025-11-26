using DynamicPriceCore.Models;

namespace DynamicPrice.Core.Services;

public record CompanyMonitoringEvent(int CompanyId, MonitoringEvent Event);
public record PriceReduceEvent(int ProductId);
//public record PriceIncreaseEvent(int OrderId);
public record PriceIncreaseEvent(ICollection<OrderItem> OrderItems);

public enum MonitoringEvent
{
    Start,
    Stop
}

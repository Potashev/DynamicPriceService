using DynamicPriceCore.Models;

namespace DynamicPrice.Core.Rabbit;

//public record CompanyMonitoringStarted(int CompanyId);
//public record CompanyMonitoringStopped(int CompanyId);
public record CompanyMonitoringEvent(int CompanyId, string Event);
public record PriceReduceEvent(int ProductId);
//public record PriceIncreaseEvent(int OrderId);
public record PriceIncreaseEvent(ICollection<OrderItem> OrderItems);

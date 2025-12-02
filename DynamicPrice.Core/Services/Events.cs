using DynamicPrice.Core.Models;

namespace DynamicPrice.Core.Services;

public record PriceReduceEvent(int ProductId);
public record PriceIncreaseEvent(ICollection<OrderItem> OrderItems);    //todo: pass id + quantity only?

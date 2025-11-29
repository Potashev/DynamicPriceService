using DynamicPriceCore.Models;

namespace DynamicPrice.Core.Services;

public record PriceReduceEvent(int ProductId);
public record PriceIncreaseEvent(ICollection<OrderItem> OrderItems);

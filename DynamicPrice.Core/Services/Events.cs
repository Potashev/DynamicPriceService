using DynamicPrice.Core.Models;

namespace DynamicPrice.Core.Services;

/// <summary>
/// Событие о необходимости снизить цену на продукт.
/// См. также <see cref="Product"/>.
/// </summary>
public record PriceReduceEvent(int ProductId);


/// <summary>
/// Событие о необходимости повысить цену на продукт после его покупки.
/// См. также <see cref="OrderItem"/>.
/// </summary>
public record PriceIncreaseEvent(int ProductId, int Quantity);

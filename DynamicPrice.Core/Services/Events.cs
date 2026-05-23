using DynamicPrice.Core.Models;

namespace DynamicPrice.Core.Services;

/// <summary>
/// Событие о необходимости снизить цену на продукт.
/// См. также <see cref="Product"/>.
/// </summary>
public record PriceReduceEvent(Guid ProductId);


/// <summary>
/// Событие о необходимости повысить цену на продукт после его покупки.
/// См. также <see cref="OrderItem"/>.
/// </summary>
public record PriceIncreaseEvent(Guid ProductId, int Quantity);

using DynamicPrice.Core.Models;

namespace DynamicPrice.Core.Services;

/// <summary>
/// Событие о необходимости снизить цену на продукт.
/// См. также <see cref="Product"/>.
/// </summary>
public record PriceReduceEvent(int ProductId);


/// <summary>
/// Событие о необходимости повысить цену на продукты, входящих в коллекцию позиций заказа.
/// См. также <see cref="OrderItem"/>.
/// </summary>
public record PriceIncreaseEvent(ICollection<OrderItem> OrderItems);	//todo: pass ids?
//public record PriceIncreaseEvent(int ProductId);

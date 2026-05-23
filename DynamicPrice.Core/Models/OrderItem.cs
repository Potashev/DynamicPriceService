using System.Text.Json.Serialization;

namespace DynamicPrice.Core.Models;

/// <summary>
/// Позиция заказа — продукт с фиксированной ценой после оформления заказа.
/// </summary>
public class OrderItem
{
	/// <summary>
	/// Идентификатор позиции заказа.
	/// </summary>
	public Guid Id { get; set; }

	/// <summary>
	/// Идентификатор заказа.
	/// </summary>
	public Guid OrderId { get; set; }

	/// <summary>
	/// Навигационное свойство заказа.
	/// </summary>
	[JsonIgnore]
	public Order Order { get; set; } = null!;

	/// <summary>
	/// Идентификатор продукта.
	/// </summary>
	public Guid ProductId { get; set; }

	/// <summary>
	/// Навигационное свойство продукта.
	/// </summary>
	public Product Product { get; set; } = null!;

	/// <summary>
	/// Фиксированная цена продукта после оформления заказа.
	/// </summary>
	public decimal ProductPrice { get; set; }

	/// <summary>
	/// Количество единиц продуктов.
	/// </summary>
	public int Quantity { get; set; }
}

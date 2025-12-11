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
	public int Id { get; set; }     //todo: make guid

	/// <summary>
	/// Идентификатор заказа.
	/// </summary>
	public int OrderId { get; set; }

	/// <summary>
	/// Навигационное свойство заказа.
	/// </summary>
	[JsonIgnore]
	public Order Order { get; set; }

	/// <summary>
	/// Идентификатор продукта.
	/// </summary>
	public int ProductId { get; set; }

	/// <summary>
	/// Навигационное свойство продукта.
	/// </summary>
	public Product Product { get; set; }

	/// <summary>
	/// Фиксированная цена продукта после оформления заказа.
	/// </summary>
	public decimal ProductPrice { get; set; }

	/// <summary>
	/// Количество единиц продуктов.
	/// </summary>
	public int Quantity { get; set; }
}

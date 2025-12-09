using System.Text.Json.Serialization;

namespace DynamicPrice.Core.Models;

/// <summary>
/// Позиция в заказе — фиксированная запись о товаре и его цене на момент оформления заказа.
/// </summary>
public class OrderItem
{
	/// <summary>
	/// Идентификатор позиции заказа.
	/// </summary>
	public int Id { get; set; }     //todo: make guid

	/// <summary>
	/// Внешний ключ на родительский заказ.
	/// </summary>
	public int OrderId { get; set; }

	/// <summary>
	/// Навигационное свойство на заказ. Игнорируется при сериализации.
	/// </summary>
	[JsonIgnore]
	public Order Order { get; set; }

	/// <summary>
	/// Идентификатор товара.
	/// </summary>
	public int ProductId { get; set; }

	/// <summary>
	/// Навигационное свойство на сущность товара (копия не сохраняется автоматически — цена хранится отдельно).
	/// </summary>
	public Product Product { get; set; }

	/// <summary>
	/// Цена товара на момент оформления заказа.
	/// </summary>
	public decimal ProductPrice { get; set; }

	/// <summary>
	/// Количество единиц товара в данной позиции.
	/// </summary>
	public int Quantity { get; set; }
}

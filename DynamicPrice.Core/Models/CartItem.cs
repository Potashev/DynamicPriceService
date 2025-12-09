using System.Text.Json.Serialization;

namespace DynamicPrice.Core.Models;

/// <summary>
/// Элемент корзины — запись о конкретном товаре в корзине пользователя.
/// </summary>
public class CartItem
{
	/// <summary>
	/// Идентификатор элемента корзины.
	/// </summary>
	public int Id { get; set; }     //todo: make guid

	/// <summary>
	/// Внешний ключ на корзину, к которой принадлежит элемент.
	/// </summary>
	public int CartId { get; set; }

	/// <summary>
	/// Навигационное свойство на родительскую корзину. Игнорируется при сериализации в JSON.
	/// </summary>
	[JsonIgnore]
	public Cart Cart { get; set; }

	/// <summary>
	/// Идентификатор товара.
	/// </summary>
	public int ProductId { get; set; }

	/// <summary>
	/// Навигационное свойство на сущность товара.
	/// </summary>
	public Product Product { get; set; }

	/// <summary>
	/// Количество единиц товара в корзине.
	/// </summary>
	public int Quantity { get; set; }
}

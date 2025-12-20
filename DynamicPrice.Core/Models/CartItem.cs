using System.Text.Json.Serialization;

namespace DynamicPrice.Core.Models;

/// <summary>
/// Элемент корзины — запись о конкретном продукте в корзине кастомера.
/// </summary>
public class CartItem
{
	/// <summary>
	/// Идентификатор элемента корзины.
	/// </summary>
	public int Id { get; set; }     //TODO: make guid

	/// <summary>
	/// Внешний ключ на корзину, к которой принадлежит элемент.
	/// </summary>
	public int CartId { get; set; }

	/// <summary>
	/// Навигационное свойство корзины.
	/// </summary>
	[JsonIgnore]
	public Cart Cart { get; set; }

	/// <summary>
	/// Идентификатор продукта.
	/// </summary>
	public int ProductId { get; set; }

	/// <summary>
	/// Навигационное свойство продукта.
	/// </summary>
	public Product Product { get; set; }

	/// <summary>
	/// Количество единиц продукта в корзине.
	/// </summary>
	public int Quantity { get; set; }
}

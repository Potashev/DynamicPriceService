namespace DynamicPrice.Core.Models;

/// <summary>
/// Корзина покупателя для конкретной компании.
/// Содержит набор элементов корзины и связь с пользователем.
/// </summary>
public class Cart
{
	/// <summary>
	/// Идентификатор корзины.
	/// </summary>
	public int CartId { get; set; } //todo: make guid?

	/// <summary>
	/// Идентификатор клиента (внешняя система/пользователь).
	/// </summary>
	public string CustomerId { get; set; }

	/// <summary>
	/// Компания, к которой относится корзина.
	/// </summary>
	public Company Company { get; set; }

	/// <summary>
	/// Коллекция элементов в корзине.
	/// </summary>
	public ICollection<CartItem> CartItems { get; set; }
}

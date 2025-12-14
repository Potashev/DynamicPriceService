namespace DynamicPrice.Core.Models;

/// <summary>
/// Корзина кастомера для конкретной компании.
/// У одного кастомера может быть много корзин в разных компаниях.
/// </summary>
public class Cart
{
	/// <summary>
	/// Идентификатор корзины.
	/// </summary>
	public int CartId { get; set; } //TODO: make guid?

	/// <summary>
	/// Идентификатор покупателя (кастомера).
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

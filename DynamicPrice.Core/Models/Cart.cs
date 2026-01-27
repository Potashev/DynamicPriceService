namespace DynamicPrice.Core.Models;

/// <summary>
/// Корзина клиента для конкретной компании.
/// У одного клиента может быть много корзин в разных компаниях.
/// </summary>
public class Cart
{
	/// <summary>
	/// Идентификатор корзины.
	/// </summary>
	public int CartId { get; set; } //TODO: make guid?

	/// <summary>
	/// Идентификатор клиента.
	/// </summary>
	public required string CustomerId { get; set; }

	/// <summary>
	/// Компания, к которой относится корзина.
	/// </summary>
	public required Company Company { get; set; }

	/// <summary>
	/// Коллекция элементов в корзине.
	/// </summary>
	public ICollection<CartItem>? CartItems { get; set; }
}

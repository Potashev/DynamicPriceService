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
	public int CartId { get; set; } //todo: make guid?

	/// <summary>
	/// Идентификатор клиента.
	/// </summary>
	public required string CustomerId { get; set; }

	/// <summary>
	/// Компания, к которой относится корзина.	//todo: fixed
	/// </summary>
	public int CompanyId { get; set; }

	/// <summary>
	/// Компания, к которой относится корзина.
	/// </summary>
	public Company Company { get; set; } = null!;   //todo: check where used and replace with CompanyId

	/// <summary>
	/// Коллекция элементов в корзине.
	/// </summary>
	public ICollection<CartItem> CartItems { get; set; } = [];
}

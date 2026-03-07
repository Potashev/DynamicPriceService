using Bogus.DataSets;
using DynamicPrice.Core.Exceptions;
using Microsoft.EntityFrameworkCore;

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
	public string CustomerId { get; set; }

	/// <summary>
	/// Компания, к которой относится корзина.	//todo: fixed
	/// </summary>
	public int CompanyId { get; set; }

	/// <summary>
	/// Компания, к которой относится корзина.
	/// </summary>
	public Company Company { get; set; } = null!;

	/// <summary>
	/// Коллекция элементов в корзине.
	/// </summary>
	public ICollection<CartItem> CartItems { get; set; } = [];

	private Cart() { }

	public Cart(string customerId, int companyId)
	{
		CustomerId = customerId;
		CompanyId = companyId;
		//Company = company;
	}

	public void AddItem(int productId)
	{
		var existingItem = CartItems
			.FirstOrDefault(ci => ci.ProductId == productId);

		if (existingItem is null)
		{
			CartItems.Add(new CartItem
			{
				Cart = this,
				ProductId = productId,
				Quantity = 1
			});
		}
		else
		{
			existingItem.Quantity += 1;
		}
	}

	public void RemoveItem(int productId)
	{
		var cartItem = CartItems
			.FirstOrDefault(ci => ci.ProductId == productId)
			?? throw new NotFoundException("Cart item not found");

		cartItem.Quantity -= 1;

		if (cartItem.Quantity == 0)
			CartItems.Remove(cartItem);
	}
}

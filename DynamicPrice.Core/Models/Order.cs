using DynamicPrice.Core.Exceptions;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;

namespace DynamicPrice.Core.Models;

/// <summary>
/// Заказ кастомера, содержащий набор продуктов с фиксированной ценой и статус обработки.
/// См. также <see cref="OrderItem"/>.
/// </summary>
[Index(nameof(Number), IsUnique = true)]
public class Order
{
	/// <summary>
	/// Идентификатор заказа.
	/// </summary>
	public int OrderId { get; set; }

	/// <summary>
	/// Номер заказа.
	/// </summary>
	[MaxLength(20)]
	public string Number { get; set; }

	/// <summary>
	/// Идентификатор кастомера, оформившего заказ.
	/// </summary>
	public string CustomerId { get; set; }

	/// <summary>
	/// Идентификатор компании, к которой относится заказ.
	/// </summary>
	public int CompanyId { get; set; }

	/// <summary>
	/// Навигационное свойство компании.
	/// </summary>
	public Company Company { get; set; } = null!;

	/// <summary>
	/// Позиции заказа - продукты с фиксированной ценой.
	/// </summary>
	public ICollection<OrderItem> OrderItems { get; } = [];

	/// <summary>
	/// Текущий статус заказа.
	/// </summary>
	public OrderStatus Status { get; set; }

	/// <summary>
	/// Дата и время создания заказа.
	/// </summary>
	public DateTime OrderDate { get; set; } = DateTime.UtcNow;

	/// <summary>
	/// Ключ получения заказа — числовой код, который может использоваться при выдаче.
	/// Доступен кастомеру и необходим для получения заказа.
	/// </summary>
	public int? ReceiveKey { get; set; }

	private Order() { }

	public Order(string customerId, int companyId)
	{
		CustomerId = customerId;
		CompanyId = companyId;

		Number = GenerateOrderNumber();
		Status = OrderStatus.Confirmed;
		OrderDate = DateTime.UtcNow;
	}

	public void AddItems(IEnumerable<CartItem> CartItems)
	{
		foreach (var item in CartItems)
		{
			var product = item.Product;

			product.ReduceQuantity(item.Quantity);

			OrderItems.Add(new OrderItem
			{
				Order = this,
				Product = product,
				ProductPrice = product.Price,
				Quantity = item.Quantity
			});
		}
	}

	public void MarkAsReady()
	{
		if (Status is not OrderStatus.Confirmed)
			throw new BusinessException("Only confirmed orders can be set to ready for receive");

		ReceiveKey = GenerateReceiveKey();
		Status = OrderStatus.Ready;
	}

	public void MarkAsCompleted()
	{
		if (Status is not OrderStatus.Ready)
			throw new BusinessException("Only ready for receive orders can be set as completed");

		foreach (var item in OrderItems)
		{
			item.Product.UpdateLastSellTime(OrderDate);
		}

		Status = OrderStatus.Completed;
		ReceiveKey = null;
	}

	public void MarkAsCanceled()
	{
		if (Status is OrderStatus.Canceled or OrderStatus.Completed)
			throw new BusinessException("Only confirmed or ready orders can be canceled.");

		foreach (var item in OrderItems)
		{
			item.Product.IncreaseQuantity(item.Quantity);
		}

		Status = OrderStatus.Canceled;
		ReceiveKey = null;
	}

	private static string GenerateOrderNumber()
	{
		var guidBytes = Guid.NewGuid().ToByteArray();

		int firstDigit = guidBytes[0] % 10;
		char letter = (char)('A' + (guidBytes[1] % 26));
		int numberPart = BitConverter.ToInt32(guidBytes, 2) & 0x7FFFFFFF;
		string lastDigits = (numberPart % 100000).ToString("D5");

		return $"{firstDigit}{letter}-{lastDigits}"; // Example: "3C-48291"
	}

	private static int GenerateReceiveKey() 
		=> RandomNumberGenerator.GetInt32(100_000, 1_000_000);
}

/// <summary>
/// Возможные статусы заказа.
/// </summary>
public enum OrderStatus
{
	/// <summary>Заказ подтверждён, в обработке.</summary>
	Confirmed,
	/// <summary>Заказ готов к выдаче.</summary>
	Ready,
	/// <summary>Заказ выдан/завершён.</summary>
	Completed,
	/// <summary>Заказ отменён.</summary>
	Canceled
}

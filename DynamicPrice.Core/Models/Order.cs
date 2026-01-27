using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

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
	public int OrderId { get; set; }    //TODO: make guid

	/// <summary>
	/// Номер заказа.
	/// </summary>
	[MaxLength(20)]
	public required string Number { get; set; }

	/// <summary>
	/// Идентификатор кастомера, оформившего заказ.
	/// </summary>
	public required string CustomerId { get; set; }

	/// <summary>
	/// Навигационное свойство компании.
	/// </summary>
	public required Company Company { get; set; }

	/// <summary>
	/// Позиции заказа - продукты с фиксированной ценой.
	/// </summary>
	public required ICollection<OrderItem> OrderItems { get; set; }

	/// <summary>
	/// Текущий статус заказа.
	/// </summary>
	public OrderStatus Status { get; set; }

	/// <summary>
	/// Дата и время создания заказа.
	/// </summary>
	public DateTime? OrderDate { get; set; }

	/// <summary>
	/// Ключ получения заказа — числовой код, который может использоваться при выдаче.
	/// Доступен кастомеру и необходим для получения заказа.
	/// </summary>
	public int ReceiveKey { get; set; }
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

using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace DynamicPrice.Core.Models;

/// <summary>
/// Заказ клиента, содержащий набор товаров и статус обработки.
/// </summary>
[Index(nameof(Number), IsUnique = true)]
public class Order
{
	/// <summary>
	/// Идентификатор заказа.
	/// </summary>
	public int OrderId { get; set; }    //todo: make guid

	/// <summary>
	/// Читабельный номер заказа (уникален).
	/// Максимальная длина — 20 символов.
	/// </summary>
	[MaxLength(20)]
	public string Number { get; set; }

	/// <summary>
	/// Идентификатор клиента, оформившего заказ.
	/// </summary>
	public string CustomerId { get; set; }

	/// <summary>
	/// Компания, из которой сделан заказ.
	/// </summary>
	public Company Company { get; set; }

	/// <summary>
	/// Элементы заказа (копии товаров с ценами на момент заказа).
	/// </summary>
	public ICollection<OrderItem> OrderItems { get; set; }

	/// <summary>
	/// Текущий статус заказа.
	/// </summary>
	public OrderStatus Status { get; set; }

	/// <summary>
	/// Дата и время создания заказа (UTC).
	/// </summary>
	public DateTime? OrderDate { get; set; }

	/// <summary>
	/// Ключ получения заказа — числовой код, который может использоваться при выдаче.
	/// </summary>
	public int ReceiveKey { get; set; }
}

/// <summary>
/// Возможные статусы заказа в системе.
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

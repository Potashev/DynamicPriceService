using DynamicPrice.Core.Services;
using System.ComponentModel.DataAnnotations.Schema;

namespace DynamicPrice.Core.Models;

/// <summary>
/// Правило ценообразования для продуктов компании.
/// Содержит параметры для повышения и снижения цен и время простоя продукта для снижения цены.
/// </summary>
public class PriceRule
{
	/// <summary>
	/// Идентификатор правила.
	/// </summary>
	public Guid Id { get; set; }

	/// <summary>
	/// Идентификатор компании.
	/// </summary>
	public Guid CompanyId { get; set; }

	/// <summary>
	/// Навигационное свойство компании, к которой относится правило.
	/// </summary>
	public Company Company { get; set; } = null!;

	/// <summary>
	/// Повышение цены продукта (в процентах).
	/// Значение 10 означает повышение на 10%.
	/// Повышение цены продукта происходит после оформления заказа.
	/// </summary>
	public double Increase { get; set; }    //todo: rename to IncreasePercent

	/// <summary>
	/// Снижение цены продукта (в процентах).
	/// Значение 10 означает снижение на 10%.
	/// Снижение цены продукта происходит при обнаружении "простоя" продукта.
	/// См. также <see cref="FindProductsToReduceService"/>.
	/// </summary>
	public double Reduction { get; set; }    //todo: rename to ReductionPercent

	/// <summary>
	/// Допустимое время "простоя" продукта. Если продукт не продавался дольше этого времени — применяется снижение цены.
	/// </summary>
	[NotMapped]
	public TimeSpan NoSellTime
	{
		get => TimeSpan.FromSeconds(NoSellSeconds);
		set => NoSellSeconds = (int)value.TotalSeconds;
	}

	/// <summary>
	/// Время "простоя" в секундах.
	/// </summary>
	public int NoSellSeconds { get; set; }
}

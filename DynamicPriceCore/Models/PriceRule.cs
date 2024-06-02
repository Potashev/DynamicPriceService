using System.Text.Json.Serialization;

namespace DynamicPriceCore.Models;

/// <summary>
/// Правило изменения цены для компании.
/// </summary>
public class PriceRule
{
	public int PriceRuleId { get; set; }

	[JsonIgnore]
	public Company Company { get; set; }

	/// <summary>
	/// Повышение цены продукта (в %).
	/// </summary>
	public int Increase { get; set; }

	/// <summary>
	/// Снижение цены продукта (в %).
	/// </summary>
	public int Reduction { get; set; }

	/// <summary>
	/// Допустимое время "простоя" продукта. Если превысили - снижаем цену (см. ReducePriceService).
	/// </summary>
	public TimeSpan? NoSellTime { get; set; } //todo: need to jsonignore?
}

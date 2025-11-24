using System.ComponentModel.DataAnnotations.Schema;

namespace DynamicPriceCore.Models;

/// <summary>
/// Правило изменения цены для компании.
/// </summary>
public class PriceRule
{
	public int PriceRuleId { get; set; }
	public Company Company { get; set; }
	public int? CompanyId { get; set; }

	/// <summary>
	/// Повышение цены продукта (в %).
	/// </summary>
	public double Increase { get; set; }

	/// <summary>
	/// Снижение цены продукта (в %).
	/// </summary>
	public double Reduction { get; set; }

    /// <summary>
    /// Допустимое время "простоя" продукта. Если превысили - снижаем цену (см. ReducePriceService).
    /// </summary>
    [NotMapped]
    public TimeSpan NoSellTime
    {
        get => TimeSpan.FromSeconds(NoSellSeconds);
        set => NoSellSeconds = (int)value.TotalSeconds;
    }
    public int NoSellSeconds { get; set; }

    //todo: think about about monitor waiting config - time before next monitoring as active company
}

namespace DynamicPrice.Core.Models;

/// <summary>
/// Компания-продавец. У каждой компании свой каталог продуктов и правило ценообразования на них.
/// См. также <see cref="Product"/> и <see cref="PriceRule"/>.
/// </summary>
public class Company
{
	/// <summary>
	/// Идентификатор компании.
	/// </summary>
	public int CompanyId { get; set; }

	/// <summary>
	/// Название компании.
	/// </summary>
	public required string Title { get; set; }
}

namespace DynamicPrice.Core.Models;

/// <summary>
/// Компания-продавец. У каждой компании свой каталог продуктов и правило изменения цен на них.
/// См. также <see cref="Product"/>.
/// См. также <see cref="PriceRule"/>.
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
	public string Title { get; set; }
}

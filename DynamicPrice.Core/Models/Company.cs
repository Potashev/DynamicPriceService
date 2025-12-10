namespace DynamicPrice.Core.Models;

/// <summary>
/// Компания-продавец. У каждой компании свой каталог продуктов и правило изменения цен на них.
/// См. также <see cref="DynamicPrice.Core.Models.Product"/>.
/// См. также <see cref="DynamicPrice.Core.Models.PriceRule"/>.
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

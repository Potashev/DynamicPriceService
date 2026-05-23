using System.ComponentModel.DataAnnotations;

namespace DynamicPrice.Shared.Contracts.ViewModels;

public class ProductViewModel : IValidatableObject
{
	public Guid ProductId { get; init; }
	public string Title { get; init; } = null!;

	[Range(0, double.MaxValue, ErrorMessage = "Price cannot be negative")]
	public decimal Price { get; init; }

	[Range(0, double.MaxValue, ErrorMessage = "Price cannot be negative")]
	public decimal MinimumPrice { get; init; }

	[Range(0, int.MaxValue, ErrorMessage = "Quantity cannot be negative")]
	public int? Quantity { get; init; }

	public string? Description { get; init; }

	public ProductStatus Status { get; init; }

	public ICollection<PriceDynamicViewModel> PriceDynamics { get; init; } = [];

	public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
	{
		if (MinimumPrice > Price)
		{
			yield return new ValidationResult(
				"Minimum price cannot be greater than price",
				new[] { nameof(MinimumPrice) }
			);
		}
	}
}

public enum ProductStatus
{
	Active,
	Archived,
}

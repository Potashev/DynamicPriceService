using System.ComponentModel.DataAnnotations;

namespace DynamicPrice.Shared.Contracts.Requests;

public class BalanceRequest
{
	[Required]
	[Range(0, double.MaxValue, ErrorMessage = "Cannot be negative")]
	public decimal ReplenishmentAmount { get; init; }
}

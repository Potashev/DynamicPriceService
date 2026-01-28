using System.ComponentModel.DataAnnotations;

namespace DynamicPrice.Shared.Contracts.Requests;

public class BalanceRequest
{
	[Required]
	[Range(0.01, 1_000_000)]
	public decimal ReplenishmentAmount { get; init; }
}

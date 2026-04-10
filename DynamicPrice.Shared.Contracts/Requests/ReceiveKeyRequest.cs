using System.ComponentModel.DataAnnotations;

namespace DynamicPrice.Shared.Contracts.Requests;

public class ReceiveKeyRequest
{
	//[Required]
	//[Range(100_000, 1_000_000, ErrorMessage = "Key must be a 6-digit number")]
	//public int? Key { get; set; }

	[Required]
	[RegularExpression(@"^\d{6}$", ErrorMessage = "Key must be a 6-digit number")]
	public string? Key { get; set; }
}

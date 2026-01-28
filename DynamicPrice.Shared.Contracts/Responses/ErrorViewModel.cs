namespace DynamicPrice.Shared.Contracts.ViewModels.Responses;

//todo: remove from shared.contracts
public class ErrorViewModel
{
	public string? RequestId { get; set; }

	public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}

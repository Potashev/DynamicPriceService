namespace DynamicPrice.Shared.Contracts.ViewModels.Responses;

public class ManagerInfoViewModel
{
	public required string Id { get; init; }
	public required string Name { get; init; }
	public required string Email { get; init; }
	public required CompanyViewModel Company { get; init; }
}

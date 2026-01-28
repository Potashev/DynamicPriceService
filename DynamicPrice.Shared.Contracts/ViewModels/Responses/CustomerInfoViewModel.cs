using DynamicPrice.Shared.Contracts.ViewModels.Entities;

namespace DynamicPrice.Shared.Contracts.ViewModels.Responses;

public class CustomerInfoViewModel
{
	public string Name { get; set; }
	public string Email { get; set; }
	public decimal? Balance { get; set; }
	public OrderViewModel[] Orders { get; set; }
}

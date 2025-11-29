namespace DynamicPrice.Core.ViewModels;

public class CustomerInfoViewModel
{
	public string Name { get; set; }
	public string Email { get; set; }
	public decimal? Balance { get; set; }
	public OrderInfoViewModel[] Orders { get; set; }    //todo: make icollection?


}

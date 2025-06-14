namespace DynamicPriceClient.ViewModels;

public class CustomerInfoViewModel
{
	public string Name { get; set; }
	public decimal Balance { get; set; }
	public OrderInfoViewModel[] Orders { get; set; }	//todo: make icollection?


}

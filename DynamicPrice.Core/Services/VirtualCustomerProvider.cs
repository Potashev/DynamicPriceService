namespace DynamicPrice.Core.Services;

public class VirtualCustomerProvider
{
	public IReadOnlyList<VirtualCustomer> Customers { get; }
	public VirtualCustomerProvider()
	{
		Customers =
		[
			new VirtualCustomer(thresholdPercent: 5),
			new VirtualCustomer(thresholdPercent: 3),
			new VirtualCustomer(thresholdPercent: 1)
		];
	}
}

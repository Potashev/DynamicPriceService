namespace DynamicPrice.Core.Services;

public class VirtualCustomerProvider
{
	private readonly List<VirtualCustomer> _customers;
	private static readonly Random _rnd = new();

	public VirtualCustomerProvider()
	{
		_customers =
		[
			new VirtualCustomer(thresholdPercent: 5),
			new VirtualCustomer(thresholdPercent: 3),
			new VirtualCustomer(thresholdPercent: 1)
		];
	}

	public VirtualCustomer GetRandom()
		=> _customers[_rnd.Next(_customers.Count)];
}

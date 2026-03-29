namespace DynamicPrice.Core.Services;

/// <summary>
/// Провайдер <see cref="VirtualCustomer"/>.
/// Предоставляет коллекцию виртуальных покупателей с различной чувствительностью к цене.
/// </summary>
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

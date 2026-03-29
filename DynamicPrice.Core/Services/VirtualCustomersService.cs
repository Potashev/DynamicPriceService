using DynamicPrice.Core.Data;
using DynamicPrice.Core.Extensions;
using DynamicPrice.Core.Models;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace DynamicPrice.Core.Services;

public class VirtualCustomersService : BackgroundService
{
	private readonly IServiceProvider _serviceProvider;
	private readonly ILogger<VirtualCustomersService> _logger;
	private readonly VirtualCustomerProvider _virtualCustomerProvider;

	public VirtualCustomersService(
		IServiceProvider serviceProvider,
		ILogger<VirtualCustomersService> logger,
		VirtualCustomerProvider provider)
	{
		_serviceProvider = serviceProvider;
		_logger = logger;
		_virtualCustomerProvider = provider;
	}

	protected override async Task ExecuteAsync(CancellationToken token)
	{
		while (!token.IsCancellationRequested)
		{
			try
			{
				await Task.Delay(VirtualCustomer.WaitNextMonitor(), token);

				using var scope = _serviceProvider.CreateScope();
				var context = scope.ServiceProvider.GetRequiredService<DynamicPriceCoreContext>();
				var publishEndpoint = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();

				var companies = await context.ActiveCompanies
					.Select(ac => ac.Company)
					.ToListAsync(token);

				if (companies.Count == 0) continue;

				var companyForMonitoring = companies.GetRandom();

				var companyProducts = await context.Products
					.Where(p => p.CompanyId == companyForMonitoring.CompanyId)
					.Where(Product.CanBeReducedExpr)
					.Include(p => p.PriceDynamics
						.OrderByDescending(pd => pd.Date)
						.Take(companyForMonitoring.PriceHistoryLimit))
					.ToArrayAsync(token);

				var virtualCustomer = _virtualCustomerProvider.Customers.GetRandom();

				var productsToBuy = virtualCustomer.MonitorProducts(companyProducts);

				var order = new Order(virtualCustomer.CustomerId, companyForMonitoring.CompanyId);

				order.AddItems(productsToBuy);
				order.MarkAsReady();
				order.MarkAsCompleted();

				context.Orders.Add(order);

				await context.SaveChangesAsync(token);

				foreach (var item in order.OrderItems)
					await publishEndpoint.Publish(
						new PriceIncreaseEvent(item.ProductId, item.Quantity),
						token);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Unhandled exception in VirtualCustomersService");
			}
		}
	}
}
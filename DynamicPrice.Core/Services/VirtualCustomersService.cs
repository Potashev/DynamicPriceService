using DynamicPrice.Core.Data;
using DynamicPrice.Core.Models;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace DynamicPrice.Core.Services;

public class VirtualCustomersService : BackgroundService
{
	private readonly IServiceProvider _serviceProvider;

	public VirtualCustomersService(IServiceProvider serviceProvider)
	{
		_serviceProvider = serviceProvider;
	}

	protected override async Task ExecuteAsync(CancellationToken token)
	{
		try
		{
			// todo: add factory?
			VirtualCustomer.CreateCustomersPool();

			while (!token.IsCancellationRequested)
			{
				await Task.Delay(VirtualCustomer.WaitNextMonitor(), token);

				using var scope = _serviceProvider.CreateScope();
				var context = scope.ServiceProvider.GetRequiredService<DynamicPriceCoreContext>();
				var publishEndpoint = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();

				var company = await context.ActiveCompanies
					.Where(ac => ac.CompanyId == 1) //todo: fixed
					.Select(ac => ac.Company)
					.FirstOrDefaultAsync(token);

				if (company is null) continue;

				//get companyprodycts
				var companyProducts = await context.Products
					.Where(p => p.CompanyId == company.CompanyId)
					.Where(Product.CanBeReducedExpr)
					.Include(p => p.PriceDynamics
						.OrderByDescending(pd => pd.Date)
						.Take(company.PriceHistoryLimit))
					.ToArrayAsync(token);

				var virtualCustomer = VirtualCustomer.GetCustomer();

				var productsToBuy = virtualCustomer.MonitorProducts(companyProducts);

				//confirm order
				var order = new Order(virtualCustomer.CustomerId, company.CompanyId);
				order.AddItems(productsToBuy);

				context.Orders.Add(order);

				await context.SaveChangesAsync(token);

				//ready to reacieve order
				order.MarkAsReady();
				await context.SaveChangesAsync(token);

				//completer order
				order.MarkAsCompleted();
				await context.SaveChangesAsync(token);

				foreach (var item in order.OrderItems)
					await publishEndpoint.Publish(
						new PriceIncreaseEvent(item.ProductId, item.Quantity),
						token);
			}
		}
		catch (Exception ex)
		{
			//_logger.LogError(ex, "Unhandled exception in FindProductsToReduceService");
		}
	}
}
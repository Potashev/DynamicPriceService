using DynamicPrice.Core.Data;
using DynamicPrice.Core.Exceptions;
using DynamicPrice.Core.Models;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace DynamicPrice.Core.Services;

public class VirtualCustomersService : BackgroundService
{
	private readonly IServiceProvider _serviceProvider;
	//private readonly ILogger<FindProductsToReduceService> _logger;

	public VirtualCustomersService(IServiceProvider serviceProvider)
	{
		_serviceProvider = serviceProvider;
	}

	protected override async Task ExecuteAsync(CancellationToken token)
	{
		try
		{
			var virtualCustomer = new VirtualCustomer 
			{
				Id = "virt-cust",
				ThresholdPercent = 5
			};

			while (!token.IsCancellationRequested)
			{
				using var scope = _serviceProvider.CreateScope();
				var context = scope.ServiceProvider.GetRequiredService<DynamicPriceCoreContext>();
				var publishEndpoint = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();	//todo: check

				//todo: use mediatr getcompanyproducts?

				var company = await context.Companies
				.FirstOrDefaultAsync(c => c.CompanyId == 1, token)	//todo: fixed
				?? throw new NotFoundException("Company not found");

				//get companyprodycts
				var companyProducts = await context.Products
					.Where(p => p.CompanyId == company.CompanyId)
					.Include(p => p.PriceDynamics
						.OrderByDescending(pd => pd.Date)
						.Take(company.PriceHistoryLimit))
					.ToArrayAsync(token);

				var productsToBuy = virtualCustomer.MonitorProducts(companyProducts);

				var cartItems = productsToBuy.Select(p => new CartItem { Product = p, Quantity = 1 });	//todo: check

				//confirm order
				var order = new Order(virtualCustomer.Id, company.CompanyId);
				order.AddItems(cartItems);

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


				await Task.Delay(TimeSpan.FromSeconds(5), token);

				//	using var scope = _serviceProvider.CreateScope();
				//	var context = scope.ServiceProvider.GetRequiredService<DynamicPriceCoreContext>();
				//	var publishEndpoint = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();

				//	var productStream = FindProductsToReduceAsync(context, token);
				//	var options = new ParallelOptions { CancellationToken = token };

				//	try
				//	{
				//		await Parallel.ForEachAsync(productStream, options, async (product, token) =>
				//		{
				//			try
				//			{
				//				await publishEndpoint.Publish(new PriceReduceEvent(product.ProductId), token);
				//			}
				//			catch (Exception ex)
				//			{
				//				_logger.LogError(ex, "Failed to publish PriceReduceEvent for ProductId {ProductId}", product.ProductId);
				//			}
				//		});
				//	}
				//	catch (OperationCanceledException) when (token.IsCancellationRequested)
				//	{
				//		break;
				//	}
				//	catch (Exception ex)
				//	{
				//		_logger.LogError(ex, "Error during parallel processing of products");
				//	}

				//	//await Task.Delay(TimeSpan.FromMilliseconds(30), token);
				//await Task.Delay(TimeSpan.FromSeconds(5), token);
			}
		}
		catch (Exception ex)
		{
			//_logger.LogError(ex, "Unhandled exception in FindProductsToReduceService");
		}
	}
}
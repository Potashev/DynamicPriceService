using DynamicPrice.Core.Data;
using DynamicPrice.Core.Models;
using DynamicPrice.Core.Services;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Prometheus;
using System.Collections.Concurrent;

public class FindProductsToReduceService : BackgroundService
{
	private readonly IServiceProvider _serviceProvider;
	private readonly ILogger<FindProductsToReduceService> _logger;

	private static readonly Histogram MonitorDuration = Metrics
	.CreateHistogram("dp_company_monitor_duration_seconds",
		"Время выполнения мониторинга компании",
		new HistogramConfiguration
		{
			LabelNames = new[] { "companyId" }
		});
	private static readonly Histogram MonitorWaitDuration = Metrics
	.CreateHistogram("dp_company_monitor_wait_seconds",
		"Время ожидания до следующего мониторинга компании",
		new HistogramConfiguration
		{
			LabelNames = new[] { "companyId" }
		});
	private readonly ConcurrentDictionary<int, DateTime> _lastMonitorEnd = new();

	public FindProductsToReduceService(IServiceProvider serviceProvider, IConfiguration config, ILogger<FindProductsToReduceService> logger)
	{
		_serviceProvider = serviceProvider;
		_logger = logger;
	}

	protected override async Task ExecuteAsync(CancellationToken token)
	{
		try
		{
			while (!token.IsCancellationRequested)
			{
				using var scope = _serviceProvider.CreateScope();
				var context = scope.ServiceProvider.GetRequiredService<DynamicPriceCoreContext>();
				var publishEndpoint = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();

				var productStream = FindProductsToReduceAsync(context, token);
				var options = new ParallelOptions { CancellationToken = token };

				try
				{
					await Parallel.ForEachAsync(productStream, options, async (product, token) =>
					{
						try
						{
							await publishEndpoint.Publish(new PriceReduceEvent(product.ProductId), token);
						}
						catch (Exception ex)
						{
							_logger.LogError(ex, "Failed to publish PriceReduceEvent for ProductId {ProductId}", product.ProductId);
						}
					});
				}
				catch (OperationCanceledException) when (token.IsCancellationRequested)
				{
					break;
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, "Error during parallel processing of products");
				}

				await Task.Delay(TimeSpan.FromSeconds(1), token);
			}
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Unhandled exception in FindProductsToReduceService");
		}
	}

	private async IAsyncEnumerable<Product> FindProductsToReduceAsync(
	DynamicPriceCoreContext context,
	CancellationToken token,
	int? productsCount = null)
	{
		var activeCompaniesIds = await context.ActiveCompanies
			.Select(ac => ac.CompanyId)
			.ToListAsync(token);

		if (activeCompaniesIds.Count == 0)
			yield break;

		var priceRulesActiveCompaniesQuery = context.PriceRules
			.AsNoTracking()
			.Where(pr => activeCompaniesIds.Contains((int)pr.CompanyId));

		var productsActiveCompaniesQuery = context.Products
			.AsNoTracking()
			.Where(p => p.CompanyId.HasValue && activeCompaniesIds.Contains(p.CompanyId.Value));

		if (productsCount.HasValue)
			productsActiveCompaniesQuery = productsActiveCompaniesQuery.Take(productsCount.Value);

		// materialize join results and filter in memory to avoid provider-specific SQL functions
		var joinList = await (from p in productsActiveCompaniesQuery
			join pr in priceRulesActiveCompaniesQuery on p.CompanyId equals pr.CompanyId
			select new { Product = p, PriceRule = pr })
			.ToListAsync(token);

		foreach (var item in joinList)
		{
			var p = item.Product;
			var pr = item.PriceRule;

			if (!p.LastSellTime.HasValue)
				continue;

			var secondsSinceLastSell = (DateTime.UtcNow - p.LastSellTime.Value).TotalSeconds;
			if (secondsSinceLastSell > pr.NoSellSeconds)
				yield return p;
		}
	}
}

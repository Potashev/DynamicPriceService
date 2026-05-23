using DynamicPrice.Core.Data;
using DynamicPrice.Core.Models;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Prometheus;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

namespace DynamicPrice.Core.Services;

/// <summary>
/// Фоновая служба мониторинга активных компаний и поиска продуктов, которые находятся в "простое".
/// Сканирует продукты активных компаний и публикует события <see cref="PriceReduceEvent"/>
/// для продуктов, которые не продавались дольше, чем разрешено правилом компании <see cref="PriceRule"/>.
/// </summary>
public class FindProductsToReduceService : BackgroundService
{
	private const int NEXT_MONITOR_MILLISECONDS = 1000;
	//private const int NEXT_MONITOR_MILLISECONDS = 5000;

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

	public FindProductsToReduceService(
		IServiceProvider serviceProvider,
		IConfiguration config,
		ILogger<FindProductsToReduceService> logger)
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
							await publishEndpoint.Publish(new PriceReduceEvent(product.Id), token);
						}
						catch (Exception ex)
						{
							_logger.LogError(ex, "Failed to publish PriceReduceEvent for ProductId {ProductId}", product.Id);
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

				await Task.Delay(TimeSpan.FromMilliseconds(NEXT_MONITOR_MILLISECONDS), token);
			}
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Unhandled exception in FindProductsToReduceService");
		}
	}

	private async IAsyncEnumerable<Product> FindProductsToReduceAsync(
		DynamicPriceCoreContext context,
		[EnumeratorCancellation] CancellationToken token,
		int? productsCount = null)
	{
		var activeCompaniesIds = await context.ActiveCompanies
			.Select(ac => ac.Id)
			.ToListAsync(token);

		if (activeCompaniesIds.Count == 0)
			yield break;

		var priceRulesActiveCompaniesQuery = context.PriceRules
			.AsNoTracking()
			.Where(pr => activeCompaniesIds.Contains(pr.CompanyId));


		var productsActiveCompaniesQuery = context.Products
			.AsNoTracking()
			.Where(p => activeCompaniesIds.Contains(p.CompanyId))
			.Where(p => p.Status == ProductStatus.Active)
			.Where(Product.CanBeReducedExpr);

		if (productsCount.HasValue)
			productsActiveCompaniesQuery = productsActiveCompaniesQuery.Take(productsCount.Value);

		var query =
			from p in productsActiveCompaniesQuery
			join pr in priceRulesActiveCompaniesQuery
				on p.CompanyId equals pr.CompanyId
			//where EF.Functions.DateDiffSecond(p.LastSellTime, DateTime.UtcNow) > pr.NoSellSeconds
			where p.LastSellTime < DateTime.UtcNow.AddSeconds(-pr.NoSellSeconds)	//todo: check
			select p;

		await foreach (var product in query.AsAsyncEnumerable().WithCancellation(token))
			yield return product;
	}
}

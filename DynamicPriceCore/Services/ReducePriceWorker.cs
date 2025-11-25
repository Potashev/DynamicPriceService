using DynamicPrice.Core.Rabbit;
using DynamicPriceCore.Data;
using DynamicPriceCore.Models;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Prometheus;
using System.Collections.Concurrent;
using System.Threading.Tasks;

public class ReducePriceWorker : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;

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

    public ReducePriceWorker(IServiceProvider serviceProvider, IConfiguration config)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken token)
    {
        try
        {
            //todo: need while?
            while (!token.IsCancellationRequested)
            {
                //using (MonitorDuration.WithLabels().NewTimer())
                //{

                using var scope = _serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<DynamicPriceCoreContext>();
                var publishEndpoint = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();

                //todo: later think about Parallel.ForEachAsync or PLINQ
                await foreach (var p in FindProductsToReduceAsync(context, token))
                {
                    await publishEndpoint.Publish(new PriceReduceEvent(p.ProductId, p.CompanyId), token);    //todo: pass companyId or just productId
                }

                //}

                //todo: remove?
                await Task.Delay(TimeSpan.FromSeconds(1), token);

                //todo: return back metrics

                //_lastMonitorEnd[companyId] = DateTime.UtcNow;

                //await Task.Delay(TimeSpan.FromSeconds(1), token);

                //if (_lastMonitorEnd.TryGetValue(companyId, out var lastEnd))
                //{
                //    var waitSeconds = (DateTime.UtcNow - lastEnd).TotalSeconds;
                //    MonitorWaitDuration.WithLabels(companyId.ToString()).Observe(waitSeconds);
                //}
            }
        }
        catch (OperationCanceledException)
        {

        }
    }

    private async IAsyncEnumerable<Product> FindProductsToReduceAsync(
    DynamicPriceCoreContext context,
    CancellationToken token,
    int? productsCount = null)
    {
        //todo: later think about configuration activeCompnay.LastMonitorTime - skip recently monitored companies if need it
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

        var query =
            from p in productsActiveCompaniesQuery
            join pr in priceRulesActiveCompaniesQuery
                on p.CompanyId equals pr.CompanyId
            where EF.Functions.DateDiffSecond(p.LastSellTime.Value, DateTime.UtcNow) > pr.NoSellSeconds
            select p;

        await foreach (var product in query.AsAsyncEnumerable().WithCancellation(token))
            yield return product;
    }
}

using DynamicPrice.Core.Rabbit;
using DynamicPrice.Core.Services;
using DynamicPriceCore.Data;
using DynamicPriceCore.Models;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Prometheus;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;
using System.Threading;

public class ReducePriceWorker : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    //private readonly ConnectionFactory _factory;
    //private IConnection? _connection;
    //private IChannel? _channel;

    private readonly IPublishEndpoint _publishEndpoint;

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

    private readonly ConcurrentDictionary<int, CancellationTokenSource> _companyMonitors = new();

    private const string ExchangeName = "company_monitoring_exchange";
    private const string QueueName = "company_monitoring_worker";

    public ReducePriceWorker(IServiceProvider serviceProvider, IPublishEndpoint publishEndpoint, IConfiguration config)
    {
        _serviceProvider = serviceProvider;
        _publishEndpoint = publishEndpoint;

        //_factory = new ConnectionFactory
        //{
        //    Uri = new Uri(config.GetConnectionString("RabbitMQ") ?? "amqp://guest:guest@localhost:5672/")
        //};
    }

    //protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    //{
    //	_connection = await _factory.CreateConnectionAsync();
    //	_channel = await _connection.CreateChannelAsync();

    //	await _channel.ExchangeDeclareAsync(exchange: ExchangeName, type: ExchangeType.Direct, durable: true);
    //	await _channel.QueueDeclareAsync(queue: QueueName, durable: true, exclusive: false, autoDelete: false);

    //	await _channel.QueueBindAsync(queue: QueueName, exchange: ExchangeName, routingKey: "company.monitoring");
    //	await _channel.QueueBindAsync(queue: QueueName, exchange: ExchangeName, routingKey: "company.monitoring.stop");

    //	//todo: check
    //	await _channel.QueueDeclareAsync(
    //		queue: "price.reduce",
    //		durable: true,
    //		exclusive: false,
    //		autoDelete: false,
    //		arguments: null);

    //	var consumer = new AsyncEventingBasicConsumer(_channel);
    //	consumer.ReceivedAsync += async (_, ea) =>
    //	{
    //		var json = Encoding.UTF8.GetString(ea.Body.ToArray());
    //		var payload = JsonSerializer.Deserialize<CompanyPayload>(json);

    //		if (payload != null)
    //		{
    //			if (ea.RoutingKey == "company.monitoring")
    //				StartMonitoringForCompany(payload.CompanyId);
    //			else if (ea.RoutingKey == "company.monitoring.stop")
    //				StopMonitoringForCompany(payload.CompanyId);
    //		}

    //		await _channel!.BasicAckAsync(ea.DeliveryTag, false);
    //	};

    //	await _channel.BasicConsumeAsync(queue: QueueName, autoAck: false, consumer: consumer);
    //}

    protected override async Task ExecuteAsync(CancellationToken token)
    {
        try
        {
            //todo: need while?
            while (!token.IsCancellationRequested)
            {
                using (MonitorDuration.WithLabels().NewTimer())
                {
                    using var scope = _serviceProvider.CreateScope();
                    var context = scope.ServiceProvider.GetRequiredService<DynamicPriceCoreContext>();

                    //todo: check - need contains companyId
                    var priceRules = await (from pr in context.PriceRules.AsNoTracking()
                                            join ac in context.ActiveCompanies.AsNoTracking()
                                                on pr.CompanyId equals ac.CompanyId
                                            select pr)
                        .ToListAsync(token);

                    await foreach (var p in FindProductsToReduceAsync(context, priceRules, token))
                    {
                        await _publishEndpoint.Publish(new PriceReduceMessage(p.ProductId, p.CompanyId), token);    //todo: pass companyId or just productId


                        //var message = new PriceReduceMessage(productId, companyId);
                        //var json = JsonSerializer.Serialize(message);
                        //var body = Encoding.UTF8.GetBytes(json);

                        //await _channel!.BasicPublishAsync(
                        //    exchange: "",
                        //    routingKey: "price.reduce",
                        //    body: body);
                    }
                }

                //todo: remove?
                await Task.Delay(TimeSpan.FromSeconds(1), token);

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
        List<PriceRule> priceRules,
        CancellationToken token,
        int? productsCount = null)
    {
        var now = DateTime.UtcNow;

        var companyIds = priceRules?
            .Where(pr => pr.CompanyId.HasValue)
            .Select(pr => pr.CompanyId!.Value)
            .Distinct()
            .ToList();

        if (companyIds == null || companyIds.Count == 0)
            yield break;

        var productsQuery = context.Products
            .AsNoTracking()
            .Where(p => p.CompanyId.HasValue && companyIds.Contains(p.CompanyId.Value));

        if (productsCount.HasValue)
            productsQuery = productsQuery.Take(productsCount.Value);

        //todo: check
        var query = from p in productsQuery
                    join pr in context.PriceRules.AsNoTracking()
                        on p.CompanyId equals pr.CompanyId
                    where pr.NoSellTime.HasValue &&
                          (
                            p.LastSellTime == null ||
                            EF.Functions.DateDiffSecond(p.LastSellTime.Value, now) >
                            (int)pr.NoSellTime.Value.TotalSeconds
                          )
                          select p;
        await foreach (var product in query.AsAsyncEnumerable().WithCancellation(token))
            yield return product;
    }

    //private async Task<List<int>> FindProductsToReduceAsyncObsolete(
    //int companyId,
    //CancellationToken token,
    //int? productsCount = null)
    //{
    //    var priceRule = await _context.PriceRules
    //        .Where(pr => pr.Company.CompanyId == companyId)
    //        .FirstOrDefaultAsync(token);

    //    if (priceRule == null)
    //        return new List<int>();

    //    var query = _context.Products.AsQueryable();

    //    if (productsCount.HasValue)
    //        query = query.Take(productsCount.Value);

    //    query = query.Where(p =>
    //        p.Company.CompanyId == companyId &&
    //        EF.Functions.DateDiffSecond(p.LastSellTime, DateTime.UtcNow) >
    //        priceRule.NoSellTime.Value.TotalSeconds);

    //    return await query
    //        .Select(p => p.ProductId)
    //        .ToListAsync(token);
    //}

    //private void StartMonitoringForCompany(int companyId)
    //{
    //    if (_companyMonitors.ContainsKey(companyId))
    //    {
    //        Console.WriteLine($"[ReducePriceWorker] Мониторинг уже запущен для {companyId}");
    //        return;
    //    }

    //    var cts = new CancellationTokenSource();
    //    if (_companyMonitors.TryAdd(companyId, cts))
    //    {
    //        Console.WriteLine($"[ReducePriceWorker] Старт мониторинга для компании {companyId}");
    //        _ = Task.Run(() => MonitorLoop(companyId, cts.Token));
    //    }
    //}

    //private void StopMonitoringForCompany(int companyId)
    //{
    //    if (_companyMonitors.TryRemove(companyId, out var cts))
    //    {
    //        Console.WriteLine($"[ReducePriceWorker] Остановка мониторинга для компании {companyId}");
    //        cts.Cancel();
    //    }
    //}

    //private async Task MonitorLoop(int companyId, CancellationToken token)
    //{
    //    try
    //    {
    //        while (!token.IsCancellationRequested)
    //        {
    //            using (MonitorDuration.WithLabels(companyId.ToString()).NewTimer())
    //            {
    //                using var scope = _serviceProvider.CreateScope();
    //                var context = scope.ServiceProvider.GetRequiredService<DynamicPriceCoreContext>();

    //                var monitor = new CompanyMonitor(context);
    //                var productsToReduce = await monitor.FindProductsToReduceAsync(companyId, token);

    //                if (productsToReduce != null)
    //                {
    //                    foreach (var productId in productsToReduce)
    //                    {
    //                        var message = new PriceReduceMessage(productId, companyId);
    //                        var json = JsonSerializer.Serialize(message);
    //                        var body = Encoding.UTF8.GetBytes(json);

    //                        await _channel!.BasicPublishAsync(
    //                            exchange: "",
    //                            routingKey: "price.reduce",
    //                            body: body);
    //                    }
    //                }
    //            }

    //            _lastMonitorEnd[companyId] = DateTime.UtcNow;

    //            await Task.Delay(TimeSpan.FromSeconds(1), token);

    //            if (_lastMonitorEnd.TryGetValue(companyId, out var lastEnd))
    //            {
    //                var waitSeconds = (DateTime.UtcNow - lastEnd).TotalSeconds;
    //                MonitorWaitDuration.WithLabels(companyId.ToString()).Observe(waitSeconds);
    //            }
    //        }
    //    }
    //    catch (OperationCanceledException)
    //    {

    //    }
    //}
}

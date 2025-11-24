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

    //private readonly IPublishEndpoint _publishEndpoint;

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

    public ReducePriceWorker(IServiceProvider serviceProvider, IConfiguration config)
    {
        _serviceProvider = serviceProvider;
        //_publishEndpoint = publishEndpoint;

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
                //using (MonitorDuration.WithLabels().NewTimer())
                //{
                using var scope = _serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<DynamicPriceCoreContext>();
                //todo: check
                var publishEndpoint = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();


                //todo: check - need contains companyId
                //var priceRules = await (from pr in context.PriceRules.AsNoTracking()
                //                        join ac in context.ActiveCompanies.AsNoTracking()
                //                            on pr.CompanyId equals ac.CompanyId
                //                        select pr)
                //    .ToListAsync(token);

                await foreach (var p in FindProductsToReduceAsync(context, token))
                {
                    await publishEndpoint.Publish(new PriceReduceMessage(p.ProductId, p.CompanyId), token);    //todo: pass companyId or just productId


                    //var message = new PriceReduceMessage(productId, companyId);
                    //var json = JsonSerializer.Serialize(message);
                    //var body = Encoding.UTF8.GetBytes(json);

                    //await _channel!.BasicPublishAsync(
                    //    exchange: "",
                    //    routingKey: "price.reduce",
                    //    body: body);
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
        // 1. PriceRules + ActiveCompanies + seconds
        var priceRulesQuery =
            from pr in context.PriceRules.AsNoTracking()
            join ac in context.ActiveCompanies.AsNoTracking()
                on pr.CompanyId equals ac.CompanyId
            //where pr.NoSellTime
            select new
            {
                pr.CompanyId,
                Seconds = pr.NoSellSeconds
            };

        var priceRules = await
            (from pr in context.PriceRules.AsNoTracking()
            join ac in context.ActiveCompanies.AsNoTracking()
                on pr.CompanyId equals ac.CompanyId
            //where pr.NoSellTime.HasValue
            select new
            {
                pr.CompanyId,
                Seconds = pr.NoSellSeconds
            }).ToListAsync();

        // 2. Собираем ID компаний с активными правилами
        var companyIds = priceRules
            .Where(pr => pr.CompanyId.HasValue)
            .Select(pr => pr.CompanyId!.Value)
            .Distinct()
            .ToList();

        if (companyIds.Count == 0)
            yield break;

        // 3. Продукты этих компаний
        var productsQuery = context.Products
            .AsNoTracking()
            .Where(p => p.CompanyId.HasValue && companyIds.Contains(p.CompanyId.Value));

        if (productsCount.HasValue)
            productsQuery = productsQuery.Take(productsCount.Value);

        var now = DateTime.UtcNow;

        //work
        var query =
            from p in productsQuery
            join pr in priceRulesQuery
                on p.CompanyId equals pr.CompanyId
            where EF.Functions.DateDiffSecond(p.LastSellTime.Value, now) > 0
            select p;

        //not work - cannot translate
        var query2 =
            from p in productsQuery
            join pr in priceRulesQuery
                on p.CompanyId equals pr.CompanyId
            where EF.Functions.DateDiffSecond(p.LastSellTime.Value, now) > pr.Seconds
            select p;


        //var query2 = productsQuery
        //    .Join(priceRules,
        //        p => p.CompanyId,
        //        pr => pr.CompanyId,
        //        (p, pr) => new { p, pr })
        //    .AsAsyncEnumerable()
        //    .Where(x => x.p.LastSellTime != null
        //                && EF.Functions.DateDiffSecond(x.p.LastSellTime.Value, now) > x.pr.Seconds)
        //    .Select(x => x.p);

        //(p, pr) new { Product = p, Rule = pr });

        // 5. Выдаём поток
        await foreach (var product in query2.AsAsyncEnumerable().WithCancellation(token))
            yield return product;
        //await foreach (var product in query2.WithCancellation(token))
        //    yield return product;

    }


    //private async IAsyncEnumerable<Product> FindProductsToReduceAsync(
    //DynamicPriceCoreContext context,
    //CancellationToken token,
    //int? productsCount = null)
    //{
    //    // 1. Загружаем правила в память + преобразуем TimeSpan → Seconds
    //    var rules = await (from pr in context.PriceRules.AsNoTracking()
    //                       join ac in context.ActiveCompanies.AsNoTracking()
    //                           on pr.CompanyId equals ac.CompanyId
    //                       where pr.NoSellTime.HasValue
    //                       select new
    //                       {
    //                           pr.CompanyId,
    //                           Seconds = (int)pr.NoSellTime.Value.TotalSeconds
    //                       })
    //                      .ToListAsync(token);

    //    if (rules.Count == 0)
    //        yield break;

    //    // 2. Список компаний, у которых есть активные правила
    //    var companyIds = rules
    //        .Where(r => r.CompanyId.HasValue)
    //        .Select(r => r.CompanyId!.Value)
    //        .Distinct()
    //        .ToList();

    //    if (companyIds.Count == 0)
    //        yield break;

    //    // 3. Продукты этих компаний
    //    var productsQuery = context.Products
    //        .AsNoTracking()
    //        .Where(p => p.CompanyId.HasValue
    //                    && companyIds.Contains(p.CompanyId.Value));

    //    if (productsCount.HasValue)
    //        productsQuery = productsQuery.Take(productsCount.Value);

    //    var now = DateTime.UtcNow;

    //    // 4. Выполняем выборку продуктов из DB
    //    //    (все сложные правила — на стороне .NET)
    //    var products = await productsQuery.ToListAsync(token);

    //    // 5. Соединяем с правилами в памяти + фильтруем
    //    foreach (var product in products)
    //    {
    //        var rule = rules.FirstOrDefault(r => r.CompanyId == product.CompanyId);
    //        if (rule == null)
    //            continue;

    //        if (product.LastSellTime == null)
    //            continue;

    //        var diffSeconds = (now - product.LastSellTime.Value).TotalSeconds;

    //        if (diffSeconds > rule.Seconds)
    //            yield return product;
    //    }
    //}

    //todo: old
    //private async IAsyncEnumerable<Product> FindProductsToReduceAsync(
    //    DynamicPriceCoreContext context,
    //    //List<PriceRule> priceRules,
    //    CancellationToken token,
    //    int? productsCount = null)
    //{

    //    var priceRulesQuery = from pr in context.PriceRules.AsNoTracking()
    //                                 join ac in context.ActiveCompanies.AsNoTracking()
    //                                     on pr.CompanyId equals ac.CompanyId
    //                                 select pr;
    //                            //.ToListAsync(token);    

    //    var companyIds = priceRulesQuery?
    //        .Where(pr => pr.CompanyId.HasValue)
    //        .Select(pr => pr.CompanyId!.Value)
    //        .Distinct()
    //        .ToList();

    //    if (companyIds == null || companyIds.Count == 0)
    //        yield break;

    //    var productsQuery = context.Products
    //        .AsNoTracking()
    //        .Where(p => p.CompanyId.HasValue && companyIds.Contains(p.CompanyId.Value));

    //    if (productsCount.HasValue)
    //        productsQuery = productsQuery.Take(productsCount.Value);

    //    var now = DateTime.UtcNow;

    //    //todo: check
    //    //var query = from p in productsQuery
    //    //            join pr in context.PriceRules.AsNoTracking()
    //    //                on p.CompanyId equals pr.CompanyId
    //    //            where pr.NoSellTime.HasValue &&
    //    //                  (
    //    //                    p.LastSellTime == null ||
    //    //                    EF.Functions.DateDiffSecond(p.LastSellTime.Value, now) >
    //    //                    (int)pr.NoSellTime.Value.TotalSeconds
    //    //                  )
    //    //                  select p;

    //    //work
    //    //var query = from p in productsQuery
    //    //            join pr in context.PriceRules.AsNoTracking()
    //    //                on p.CompanyId equals pr.CompanyId
    //    //            where pr.NoSellTime.HasValue
    //    //            select p;

    //    //not work
    //    //var query =
    //    //    from p in productsQuery
    //    //    join pr in context.PriceRules.AsNoTracking()
    //    //        on p.CompanyId equals pr.CompanyId
    //    //    where p.LastSellTime < now - pr.NoSellTime
    //    //    select p;

    //    //var query =
    //    //    from p in productsQuery
    //    //    join pr in context.PriceRules.AsNoTracking()
    //    //        on p.CompanyId equals pr.CompanyId
    //    //    where pr.NoSellTime.HasValue &&
    //    //          (
    //    //              p.LastSellTime == null ||
    //    //              EF.Functions.DateDiffSecond(p.LastSellTime.Value, now) >
    //    //              (int)pr.NoSellTime.Value.TotalSeconds
    //    //          )
    //    //    select p;

    //    var query =
    //        from p in productsQuery
    //        join pr in context.PriceRules.AsNoTracking()
    //            on p.CompanyId equals pr.CompanyId
    //        where EF.Functions.DateDiffSecond(p.LastSellTime.Value, now) >
    //                 (int)pr.NoSellTime.Value.TotalSeconds
    //        select p;

    //    //var productsToReduce = query.ToList();

    //    //await foreach (var product in query.AsAsyncEnumerable().WithCancellation(token))
    //    await foreach (var product in query.AsAsyncEnumerable().WithCancellation(token))
    //        yield return product;
    //}

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

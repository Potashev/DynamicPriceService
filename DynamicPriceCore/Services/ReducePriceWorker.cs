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
                var publishEndpoint = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();

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

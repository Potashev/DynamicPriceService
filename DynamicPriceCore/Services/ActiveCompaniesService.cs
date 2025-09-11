using DynamicPriceCore.Data;
using DynamicPriceCore.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Collections.Concurrent;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;
using DynamicPrice.Core.Rabbit;

namespace DynamicPriceCore.Services;

/// <summary>
/// Список компаний для мониторинга "простоя".
/// </summary>
//public class ActiveCompaniesService : IActiveCompaniesService
//{
//	//private List<Company> _activeCompanies = new List<Company>();

//	//private List<Company> _companiesToAdd = new List<Company>();
//	//private List<Company> _companiesToRemove = new List<Company>();

//	//private readonly DynamicPriceCoreContext _context;
//	private readonly IServiceProvider _serviceProvider;

//	private List<int> _activeCompanies = new List<int>();
//	private List<int> _companiesToAdd = new List<int>();
//	private List<int> _companiesToRemove = new List<int>();

//	public ActiveCompaniesService(IServiceProvider serviceProvider)
//	{
//		//_context = context;
//		_serviceProvider = serviceProvider;
//	}

//	public IEnumerable<Company> GetActiveCompanies()
//	{
//		//var companies = _context.Companies
//		//	.Where(c => _activeCompanies.Contains(c.CompanyId))
//		//	.ToList();
//		//return companies;


//		if (!_activeCompanies.Any()) return Enumerable.Empty<Company>();

//		//bad
//		using var scope = _serviceProvider.CreateScope();
//		var context = scope.ServiceProvider.GetRequiredService<DynamicPriceCoreContext>();
//		return context.Companies
//			.Where(c => _activeCompanies.Contains(c.CompanyId))
//			.ToList();
//	}
//	public void AddRequest(int companyId) => _companiesToAdd.Add(companyId);
//	//public void RemoveRequest(Company company) => _companiesToRemove.Add(company);
//	public void RemoveRequest(int companyId) => _companiesToRemove.Add(companyId);

//	//public void RemoveRequest(Company company) => _companiesToRemove.RemoveAll(c => c.CompanyId == company.CompanyId);
//	//public bool IsActive(Company company) => _activeCompanies.Any(c => c.CompanyId == company.CompanyId);
//	public bool IsActive(int companyId) => _activeCompanies.Any(cid => cid == companyId);

//	//todo: looks like not good. The issue changing collection when foreach in reduceprice job. Make better solution or check to avoid collision
//	public void HandleRequests()
//	{
//		//todo: check
//		_activeCompanies.AddRange(_companiesToAdd);
//		_activeCompanies.RemoveAll(cid => _companiesToRemove.Any(rid => rid == cid));
//		_companiesToAdd.Clear();
//		_companiesToRemove.Clear();
//	}
//}

public class ActiveCompaniesService : IActiveCompaniesService, IAsyncDisposable
{
	private readonly IServiceProvider _serviceProvider;
	private readonly ConnectionFactory _factory;
	private IConnection? _connection;
	private IChannel? _channel;

	private readonly ConcurrentDictionary<int, bool> _activeCompanies = new();

	private const string ExchangeName = "company_monitoring_exchange";
	private const string QueueName = "company_monitoring_queue";

	public ActiveCompaniesService(IServiceProvider serviceProvider, IConfiguration config)
	{
		_serviceProvider = serviceProvider;

		// Конфиг читаем из appsettings.json
		var rabbitMqConnStr = config.GetConnectionString("RabbitMQ") ?? "amqp://guest:guest@localhost:5672/";
		_factory = new ConnectionFactory
		{
			Uri = new Uri(rabbitMqConnStr)
		};

		// Запускаем consumer на фоне
		_ = Task.Run(StartConsumerAsync);
	}

	public async Task StartConsumerAsync()
	{
		_connection = await _factory.CreateConnectionAsync();
		_channel = await _connection.CreateChannelAsync();

		await _channel.ExchangeDeclareAsync(exchange: ExchangeName, type: ExchangeType.Direct, durable: true);
		await _channel.QueueDeclareAsync(queue: QueueName, durable: true, exclusive: false, autoDelete: false);
		await _channel.QueueBindAsync(queue: QueueName, exchange: ExchangeName, routingKey: "company.start");
		await _channel.QueueBindAsync(queue: QueueName, exchange: ExchangeName, routingKey: "company.stop");

		var consumer = new AsyncEventingBasicConsumer(_channel);
		consumer.ReceivedAsync += async (sender, ea) =>
		{
			try
			{
				var json = Encoding.UTF8.GetString(ea.Body.ToArray());
				if (ea.RoutingKey == "company.start")
				{
					var evt = JsonSerializer.Deserialize<CompanyMonitoringStarted>(json);
					if (evt != null) _activeCompanies.TryAdd(evt.CompanyId, true);
				}
				else if (ea.RoutingKey == "company.stop")
				{
					var evt = JsonSerializer.Deserialize<CompanyMonitoringStopped>(json);
					if (evt != null) _activeCompanies.TryRemove(evt.CompanyId, out _);
				}

				await _channel!.BasicAckAsync(ea.DeliveryTag, multiple: false);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"[ActiveCompaniesService] Error handling message: {ex}");
				// Можно сделать BasicNack с requeue=true
			}
		};

		await _channel.BasicConsumeAsync(queue: QueueName, autoAck: false, consumer: consumer);
	}

	public IEnumerable<Company> GetActiveCompanies()
	{
		if (_activeCompanies.IsEmpty) return Enumerable.Empty<Company>();

		var activeCompanyIds = _activeCompanies.Keys.ToArray();

		using var scope = _serviceProvider.CreateScope();
		var context = scope.ServiceProvider.GetRequiredService<DynamicPriceCoreContext>();

		return context.Companies
			.Where(c => activeCompanyIds.Contains(c.CompanyId))
			.ToList();
	}

	public bool IsActive(int companyId) => _activeCompanies.ContainsKey(companyId);

	public async ValueTask DisposeAsync()
	{
		if (_channel != null) await _channel.CloseAsync();
		if (_connection != null) await _connection.CloseAsync();
	}

	public void AddRequest(int companyId)
	{
		throw new NotImplementedException();
	}

	public void RemoveRequest(int companyId)
	{
		throw new NotImplementedException();
	}

	public void HandleRequests()
	{
		//throw new NotImplementedException();
	}
}

public interface IActiveCompaniesService
{
	IEnumerable<Company> GetActiveCompanies();
	//void AddRequest(Company company);
	//void RemoveRequest(Company company);
	//bool IsActive(Company company);

	//IEnumerable<int> GetActiveCompanies();
	bool IsActive(int companyId);
	void AddRequest(int companyId);
	void RemoveRequest(int companyId);
	void HandleRequests();
}
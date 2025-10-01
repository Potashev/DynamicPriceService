//using BenchmarkDotNet.Attributes;
//using BenchmarkDotNet.Running;
//using DynamicPrice.Core.Services;
//using DynamicPriceCore.Data;
//using DynamicPriceCore.Models;
//using Microsoft.EntityFrameworkCore;
//using System.Threading.Tasks;

//public class MonitorBenchmark
//{
//	private DynamicPriceCoreContext _context = null!;
//	private CompanyMonitor _monitor = null!;
//	private int _companyId;

//	[Params(10, 100, 1000, 10000)]
//	public int ProductCount; // разные размеры

//	[GlobalSetup]
//	public void Setup()
//	{
//		//var options = new DbContextOptionsBuilder<DynamicPriceCoreContext>()
//		//	.UseInMemoryDatabase(Guid.NewGuid().ToString())
//		//	.Options;

//		//_context = new DynamicPriceCoreContext(options);

//		//// Заполняем тестовыми данными
//		//var company = new Company { CompanyId = 1, Name = "Test" };
//		//_context.Companies.Add(company);
//		//_context.PriceRules.Add(new PriceRule
//		//{
//		//	Company = company,
//		//	NoSellTime = TimeSpan.FromSeconds(60)
//		//});

//		//for (int i = 0; i < ProductCount; i++)
//		//{
//		//	_context.Products.Add(new Product
//		//	{
//		//		ProductId = i + 1,
//		//		Company = company,
//		//		LastSellTime = DateTime.UtcNow.AddMinutes(-i) // часть продуктов "устарела"
//		//	});
//		//}

//		//_context.SaveChanges();
//		//_monitor = new CompanyMonitor(_context);
//		//_companyId = company.CompanyId;

//		_monitor = new CompanyMonitor(_context);
//	}

//	[Benchmark]
//	public async Task RunMonitorIteration()
//	{
//		await _monitor.FindProductsToReduceAsync(_companyId, CancellationToken.None);
//	}
//}

//// запуск
//public static class Program
//{
//	public static void Main(string[] args) =>
//		BenchmarkRunner.Run<MonitorBenchmark>();
//}

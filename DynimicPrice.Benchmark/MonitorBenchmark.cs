using BenchmarkDotNet.Attributes;
using DynamicPrice.Core.Services;
using DynamicPriceCore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace DynamicPrice.Benchmarks;

[MemoryDiagnoser] // можно оставить, даже если интересует только время
public class MonitorBenchmark
{
	private DynamicPriceCoreContext _context = null!;
	private CompanyMonitor _monitor = null!;
	private int _companyId;
	private readonly string _connectionString = "Server=(localdb)\\mssqllocaldb;Database=DynamicPriceDb;Trusted_Connection=True;MultipleActiveResultSets=true";

	[Params(1)]
	public int CompanyId;

	[Params(5, 10, 25, 50, 100)]
	public int ProductsCount;

	[GlobalSetup]
	public void Setup()
	{
		var options = new DbContextOptionsBuilder<DynamicPriceCoreContext>()
			.UseSqlServer(_connectionString)
			.Options;

		_context = new DynamicPriceCoreContext(options);

		_monitor = new CompanyMonitor(_context);
		_companyId = CompanyId;
	}

	[Benchmark]
	public async Task RunMonitorIteration()
	{
		await _monitor.FindProductsToReduceAsync(
			_companyId,
			CancellationToken.None,
			ProductsCount);
	}
}

using System.Diagnostics;

namespace DynamicPrice.Core.Benchmark;

public static class DPBenchmark
{
	private static Stopwatch _stopwatch = new Stopwatch();
	public static TimeSpan Time { get; private set; }
	public static bool IsStarted { get; private set; } = false;
	public static bool IsFinished { get; private set; } = false;

	//private static readonly string LogFileDir = Path.Combine(
	//	AppDomain.CurrentDomain.BaseDirectory, 
	//	"Benchmark"
	//);

	private static readonly string LogFilePath = Path.Combine(
		AppDomain.CurrentDomain.BaseDirectory, 
		"benchmark_results.txt"
	);

	public static void Start()
	{
		_stopwatch.Restart();
		IsStarted = true;
	}

	public static void Stop()
	{
		_stopwatch.Stop();
		Time = _stopwatch.Elapsed;
		IsStarted = false;
		IsFinished = true;
	}

	public static void Report()
	{
		var message = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Время выполнения: {Time.TotalSeconds:F5} сек.";
		//Console.WriteLine("[Benchmark] " + message);

		try
		{
			File.AppendAllText(LogFilePath, message + Environment.NewLine);
		}
		catch (Exception ex)
		{
			//Console.WriteLine($"[Benchmark] Ошибка записи в лог: {ex.Message}");
		}
	}
}

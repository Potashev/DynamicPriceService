namespace DynamicPrice.Client.Common;

public class PriceHubClientOptions
{
	public string HubUrl { get; set; } = "/priceHub";
}

public static class PriceHubClientExtensions
{
	//public static IApplicationBuilder UsePriceMonitorStaticFiles(this IApplicationBuilder app)
	//{
	//	// Подключаем wwwroot из общей библиотеки
	//	app.UseStaticFiles(new StaticFileOptions
	//	{
	//		FileProvider = new ManifestEmbeddedFileProvider(typeof(PriceHubClientExtensions).Assembly, "wwwroot"),
	//		RequestPath = ""
	//	});

	//	return app;
	//}
}

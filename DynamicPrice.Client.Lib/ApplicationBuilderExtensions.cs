using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.FileProviders;

namespace DynamicPrice.Client.Common;

public class PriceHubClientOptions
{
	public string HubUrl { get; set; } = "/priceHub";
}

//public static class ApplicationBuilderExtensions
//{
//	public static IApplicationBuilder UsePriceMonitorStaticFiles(this IApplicationBuilder app)
//	{
//		// Подключаем wwwroot из общей библиотеки
//		app.UseStaticFiles(new StaticFileOptions
//		{
//			FileProvider = new ManifestEmbeddedFileProvider(typeof(ApplicationBuilderExtensions).Assembly, "wwwroot"),
//			RequestPath = ""
//		});

//		return app;
//	}
//}

public static class ApplicationBuilderExtensions
{
	public static IApplicationBuilder UseDynamicPriceClientCommon(this IApplicationBuilder app)
	{
		//app.UseStaticFiles(new StaticFileOptions
		//{
		//	FileProvider = new ManifestEmbeddedFileProvider(typeof(ApplicationBuilderExtensions).Assembly, "wwwroot"),
		//	RequestPath = "/dp-common"
		//});

		return app;
	}

	//public static IApplicationBuilder UseDynamicPriceClientCommon(this IApplicationBuilder app)
	//{
	//	// Подключаем wwwroot из общей библиотеки
	//	app.UseStaticFiles(new StaticFileOptions
	//	{
	//		FileProvider = new ManifestEmbeddedFileProvider(typeof(ApplicationBuilderExtensions).Assembly, "wwwroot"),
	//		RequestPath = ""
	//	});

	//	return app;
	//}
}

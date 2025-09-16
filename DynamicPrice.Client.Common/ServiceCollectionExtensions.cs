using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DynamicPrice.Client.Common;
public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddClientCommon(this IServiceCollection services)
	{
		services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
		services.TryAddScoped<IAuthTokenStore, CookiesAuthTokenStore>();
		services.AddTransient<AuthHeaderHandler>();

		return services;
	}
}

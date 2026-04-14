using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DynamicPrice.Client.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddClientCommon(this IServiceCollection services)
	{
		services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
		//services.TryAddScoped<IAuthTokenStore, CookiesAuthTokenStore>();
		services.TryAddScoped<IAuthTokenStore, SessionAuthTokenStore>();
		services.TryAddScoped<ICurrentUserService, CurrentUserService>();
		services.TryAddTransient<AuthHeaderHandler>();

		return services;
	}
}

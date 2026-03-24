using DynamicPrice.Client.Infrastructure;
using DynamicPrice.Client.Infrastructure.Extensions;
using DynamicPrice.Manager.ApiClients;
using Microsoft.AspNetCore.Localization;
using Refit;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddSession(options =>
{
	options.IdleTimeout = TimeSpan.FromMinutes(30);
	options.Cookie.HttpOnly = true;
	options.Cookie.IsEssential = true;
	options.Cookie.Name = "Manager.Session";   //for using manager and customer in one browser
});

builder.Services.AddClientCommon();

var baseUrl = builder.Configuration["ApiSettings:BaseUrl"]
	?? throw new InvalidOperationException("Missing configuration: ApiSettings:BaseUrl");

builder.Services.AddRefitClient<ICoreApiClient>()
	.ConfigureHttpClient(client =>
	{
		client.BaseAddress = new Uri(baseUrl);
		client.DefaultRequestHeaders.Add("Accept", "application/json");
		client.Timeout = TimeSpan.FromMinutes(10);
	})
	.AddHttpMessageHandler<AuthHeaderHandler>();

var app = builder.Build();

var culture = new CultureInfo("ru-RU");
var localizationOptions = new RequestLocalizationOptions
{
	DefaultRequestCulture = new RequestCulture(culture),
	SupportedCultures = new[] { culture },
	SupportedUICultures = new[] { culture }
};

app.UseRequestLocalization(localizationOptions);

if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.MapControllerRoute(
	name: "default",
		pattern: "{controller=Auth}/{action=LoginManager}");

app.Run();

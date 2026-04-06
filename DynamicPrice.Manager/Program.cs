using DynamicPrice.Client.Infrastructure;
using DynamicPrice.Client.Infrastructure.Extensions;
using DynamicPrice.Customer.Extension;
using DynamicPrice.Manager.ApiClients;
using Refit;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews(options =>
{
	options.Filters.Add<ApiExceptionFilter>();
});

builder.Services.AddSession(options =>
{
	options.IdleTimeout = TimeSpan.FromMinutes(30);
	options.Cookie.HttpOnly = true;
	options.Cookie.IsEssential = true;
	options.Cookie.Name = "Manager.Session";
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

if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
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

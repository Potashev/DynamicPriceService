using DynamicPrice.Client.Infrastructure;
using DynamicPrice.Manager.ApiClients;
using Refit;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddSession(options =>
{
	options.IdleTimeout = TimeSpan.FromMinutes(30);
	options.Cookie.HttpOnly = true;
	options.Cookie.IsEssential = true;
	options.Cookie.Name = "Manager.Session";   //for using manager and customer in one browser
});

var baseUrl = builder.Configuration["ApiSettings:BaseUrl"];

builder.Services.AddClientCommon();

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
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.MapControllerRoute(
	name: "default",
		pattern: "{controller=Auth}/{action=Login}");

app.Run();

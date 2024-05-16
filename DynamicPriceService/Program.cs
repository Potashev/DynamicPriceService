using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using DynamicPriceService.Data;
using DynamicPriceService.Services;
using Quartz;
using DynamicPriceService.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<DynamicPriceServiceContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("DynamicPriceServiceContext") ?? throw new InvalidOperationException("Connection string 'DynamicPriceServiceContext' not found.")));

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddSingleton<IActiveCompaniesService, ActiveCompaniesService>();   //singleton - is it right?

builder.Services.AddQuartz(q => q.AddJobAndTrigger<ReducePriceJob>(builder.Configuration));
builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

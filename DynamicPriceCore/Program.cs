using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using DynamicPriceCore.Data;
using DynamicPriceCore.Controllers;
using Quartz;
using DynamicPriceCore.Services;
using DynamicPriceCore.Extensions;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<DynamicPriceCoreContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("DynamicPriceCoreContext") ?? throw new InvalidOperationException("Connection string 'DynamicPriceCoreContext' not found.")));

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowSpecificOrigins",
		policy =>
		{
			policy.WithOrigins("https://localhost:7022")  // Добавляем клиентский адрес
				  .AllowAnyHeader()
				  .AllowAnyMethod()
				  .AllowCredentials();  // Разрешаем отправлять куки и аутентификационные данные
		});
});

builder.Services.AddSignalR(); // Добавляем поддержку SignalR

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<Program>());

builder.Services.AddSingleton<IActiveCompaniesService, ActiveCompaniesService>();
builder.Services.AddTransient<IIncreasePriceService, IncreasePriceService>();	//todo: change

builder.Services.AddQuartz(q => q.AddJobAndTrigger<ReducePriceJob>(builder.Configuration));
builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Включаем CORS для всех маршрутов или только для хаба
app.UseCors("AllowSpecificOrigins");
// Регистрируем хаб SignalR
app.MapHub<PriceHub>("/priceHub"); // Убедитесь, что маршрут хаба корректен

app.Run();

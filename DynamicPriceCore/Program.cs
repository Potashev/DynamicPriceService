using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using DynamicPriceCore.Data;
using Quartz;
using DynamicPriceCore.Services;
using DynamicPriceCore.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using System;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<DynamicPriceCoreContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("DynamicPriceCoreContext") ?? throw new InvalidOperationException("Connection string 'DynamicPriceCoreContext' not found.")));


//var t = builder.Services;

//builder.Services.AddDefaultIdentity<IdentityUser>(options =>
//	options.SignIn.RequireConfirmedAccount = true)
//	.AddEntityFrameworkStores<ApplicationDbContext>();


//builder.Services.AddIdentity<IdentityUser, IdentityRole>()
//	.AddEntityFrameworkStores<DynamicPriceCoreContext>()
//	.AddDefaultTokenProviders();

//builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
//	options.SignIn.RequireConfirmedAccount = true)
//	.AddEntityFrameworkStores<DynamicPriceCoreContext>();

//builder.Services.Configure<IdentityOptions>(options =>
//{
//	// Password settings.
//	options.Password.RequireDigit = true;
//	options.Password.RequireLowercase = true;
//	options.Password.RequireNonAlphanumeric = true;
//	options.Password.RequireUppercase = true;
//	options.Password.RequiredLength = 6;
//	options.Password.RequiredUniqueChars = 1;

//	// Lockout settings.
//	options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
//	options.Lockout.MaxFailedAccessAttempts = 5;
//	options.Lockout.AllowedForNewUsers = true;

//	// User settings.
//	options.User.AllowedUserNameCharacters =
//	"abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
//	options.User.RequireUniqueEmail = false;
//});



builder.Services.AddIdentity<IdentityUser, IdentityRole>()
	.AddEntityFrameworkStores<DynamicPriceCoreContext>()
	.AddDefaultTokenProviders();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
	.AddJwtBearer(options =>
	{
		options.TokenValidationParameters = new TokenValidationParameters
		{
			ValidateIssuer = true,
			ValidateAudience = true,
			ValidateLifetime = true,
			ValidateIssuerSigningKey = true,
			ValidIssuer = "TestIssuer",
			ValidAudience = "TestAudience",
			IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("SuperSecretKey123!"))
		};
	});

builder.Services.AddAuthorization(options =>
{
	options.AddPolicy("ManagerPolicy", policy => policy.RequireRole("Manager"));
	options.AddPolicy("CustomerPolicy", policy => policy.RequireRole("Customer"));
});



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

	app.ApplyMigrations();
}

app.UseHttpsRedirection();

app.UseAuthentication();	//todo: is it right?
app.UseAuthorization();

app.MapControllers();

// Включаем CORS для всех маршрутов или только для хаба
app.UseCors("AllowSpecificOrigins");
// Регистрируем хаб SignalR
app.MapHub<PriceHub>("/priceHub"); // Убедитесь, что маршрут хаба корректен

app.Run();

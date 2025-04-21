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




//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//	.AddJwtBearer(options =>
//	{
//		options.TokenValidationParameters = new TokenValidationParameters
//		{
//			ValidateIssuer = true,
//			ValidateAudience = true,
//			ValidateLifetime = true,
//			ValidateIssuerSigningKey = true,
//			ValidIssuer = "TestIssuer",
//			ValidAudience = "TestAudience",
//			IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("SuperSecretKey123!"))
//		};
//	});

//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//	.AddJwtBearer(options =>
//	{
//		options.TokenValidationParameters = new TokenValidationParameters
//		{
//			ValidateIssuerSigningKey = true,
//			IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
//			ValidateIssuer = true,
//			ValidateAudience = true,
//			ValidIssuer = builder.Configuration["Jwt:Issuer"],
//			ValidAudience = builder.Configuration["Jwt:Audience"]
//		};
//	});

//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//	.AddJwtBearer(options =>
//	{
//		options.TokenValidationParameters = new TokenValidationParameters
//		{
//			ValidateIssuerSigningKey = true,
//			IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
//			ValidateIssuer = true,
//			ValidateAudience = true,
//			ValidIssuer = builder.Configuration["Jwt:Issuer"],
//			ValidAudience = builder.Configuration["Jwt:Audience"],
//			ValidateLifetime = true, // Включаем проверку времени жизни токена
//			ClockSkew = TimeSpan.Zero // Убираем временную задержку
//		};

//		Console.WriteLine($"🔍 Jwt:Key = {builder.Configuration["Jwt:Key"]}");
//		Console.WriteLine($"🔍 Jwt:Issuer = {builder.Configuration["Jwt:Issuer"]}");
//		Console.WriteLine($"🔍 Jwt:Audience = {builder.Configuration["Jwt:Audience"]}");

//	});


//builder.Services.AddAuthorization(options =>
//{

//	options.InvokeHandlersAfterFailure = true;

//	options.AddPolicy("ManagerPolicy", policy => policy.RequireRole("Manager"));
//	options.AddPolicy("CustomerPolicy", policy => policy.RequireRole("Customer"));
//});


Console.WriteLine("🚀 Перед AddAuthentication()...");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
	.AddJwtBearer(options =>
	{

		options.IncludeErrorDetails = true;

		options.Events = new JwtBearerEvents
		{
			OnMessageReceived = context =>
			{
				Console.WriteLine($"🔹 JWT Token получен: {context.Token}");
				return Task.CompletedTask;
			},
			OnAuthenticationFailed = context =>
			{
				Console.WriteLine($"❌ Ошибка аутентификации: {context.Exception.Message}");
				return Task.CompletedTask;
			}
		};

		Console.WriteLine("✅ AddJwtBearer вызван!");

		options.TokenValidationParameters = new TokenValidationParameters
		{
			//ValidateIssuerSigningKey = true,
			//IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
			//ValidateIssuer = true,
			//ValidateAudience = true,
			//ValidIssuer = builder.Configuration["Jwt:Issuer"],
			//ValidAudience = builder.Configuration["Jwt:Audience"],
			//ValidateLifetime = true,
			//ClockSkew = TimeSpan.Zero
			ValidateIssuerSigningKey = true,
			IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),

			ValidateIssuer = true,
			ValidateAudience = true,
			ValidIssuer = "TestIssuer",       // ← ДОЛЖНО совпадать с "iss" в токене
			ValidAudience = "TestAudience",   // ← как "aud"

			ValidateLifetime = true,
			ClockSkew = TimeSpan.Zero
		};
	});

Console.WriteLine("🚀 После AddAuthentication()...");

builder.Services.AddAuthorization();


builder.Services.AddIdentity<IdentityUser, IdentityRole>()
	.AddEntityFrameworkStores<DynamicPriceCoreContext>()
	.AddDefaultTokenProviders();


//builder.Services.AddAuthorization(options =>
//{
//	options.AddPolicy("ManagerPolicy", policy => policy.RequireRole("Manager"));
//	options.AddPolicy("CustomerPolicy", policy => policy.RequireRole("Customer"));
//});

//todo: check
//builder.Services.AddDistributedMemoryCache();

//builder.Services.AddSession(options =>
//{
//	options.IdleTimeout = TimeSpan.FromSeconds(10);
//	options.Cookie.HttpOnly = true;
//	options.Cookie.IsEssential = true;
//});


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
//builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(options =>
{
	options.SwaggerDoc("v1", new() { Title = "Your API", Version = "v1" });

	options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
	{
		Name = "Authorization",
		Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
		Scheme = "Bearer",
		BearerFormat = "JWT",
		In = Microsoft.OpenApi.Models.ParameterLocation.Header,
		Description = "Введите токен как: Bearer {ваш_токен}"
	});

	options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
	{
		{
			new Microsoft.OpenApi.Models.OpenApiSecurityScheme
			{
				Reference = new Microsoft.OpenApi.Models.OpenApiReference
				{
					Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
					Id = "Bearer"
				}
			},
			Array.Empty<string>()
		}
	});
});

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

//app.UseAuthentication();	//todo: is it right?
//app.UseAuthorization();

app.Use(async (context, next) =>
{
	Console.WriteLine($"🔍 Входящие заголовки: {string.Join(", ", context.Request.Headers.Select(h => $"{h.Key}: {h.Value}"))}");
	await next();
});
app.UseAuthentication();  // Обязательно перед Authorization!

app.Use(async (context, next) =>
{
	Console.WriteLine("Мидлвейр после auth:");
	Console.WriteLine($"🔍 Входящие заголовки: {string.Join(", ", context.Request.Headers.Select(h => $"{h.Key}: {h.Value}"))}");
	await next();
});

// проверить почему токен не проходит валидацию в этом middleware (детальнее ошибку посмотреть и загуглить)
// детальнее посмотреть использование mw этого и возможные проблемы (также что нет у меня userouting и useendpoints которые нужны вроде как)
// попробовать больше логов добавить для детализации ошибки и проверить конфигурацию addauth addauthoriz
app.UseAuthorization();

app.Use(async (context, next) =>
{
	Console.WriteLine("Финалка:");
	Console.WriteLine($"🔍 Входящие заголовки: {string.Join(", ", context.Request.Headers.Select(h => $"{h.Key}: {h.Value}"))}");
	await next();
});

//app.UseSession();	//todo: check

app.MapControllers();

// Включаем CORS для всех маршрутов или только для хаба
app.UseCors("AllowSpecificOrigins");
// Регистрируем хаб SignalR
app.MapHub<PriceHub>("/priceHub"); // Убедитесь, что маршрут хаба корректен

app.Run();

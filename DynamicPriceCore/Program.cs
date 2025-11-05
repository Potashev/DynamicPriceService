using DynamicPrice.Core.Services;
using DynamicPriceCore.Data;
using DynamicPriceCore.Models;
using DynamicPriceCore.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Prometheus;
using Quartz;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<DynamicPriceCoreContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("DynamicPriceDb") ?? throw new InvalidOperationException("Connection string 'DynamicPriceDb' not found.")));

builder.Services.AddDbContext<IdentityContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("IdentityDb") ?? throw new InvalidOperationException("Connection string 'IdentityDb' not found.")));


builder.Services.AddAuthentication(options =>
{
	options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
	options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
	options.TokenValidationParameters = new TokenValidationParameters
	{
		ValidateIssuerSigningKey = true,
		IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),

		ValidateIssuer = true,
		ValidateAudience = true,
		ValidIssuer = "TestIssuer",
		ValidAudience = "TestAudience",

		ValidateLifetime = true,
		ClockSkew = TimeSpan.Zero
	};

	options.Events = new JwtBearerEvents
	{
		OnMessageReceived = context =>
		{
			var accessToken = context.Request.Cookies["tests"];
			if (!string.IsNullOrEmpty(accessToken))
			{
				context.Token = accessToken;
			}
			return Task.CompletedTask;
		}
	};
});


builder.Services.AddAuthorization(options =>
{
	options.AddPolicy("ManagerPolicy", policy => policy.RequireRole("Manager"));
	options.AddPolicy("CustomerPolicy", policy => policy.RequireRole("Customer"));
});

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
	.AddEntityFrameworkStores<IdentityContext>()
	.AddDefaultTokenProviders();

builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowSpecificOrigins",
		policy =>
		{
			// customer
			policy.WithOrigins("https://localhost:7022")
				  .AllowAnyHeader()
				  .AllowAnyMethod()
				  .AllowCredentials();

			// manager
			policy.WithOrigins("https://localhost:7183")
				  .AllowAnyHeader()
				  .AllowAnyMethod()
				  .AllowCredentials();
		});
});

builder.Services.AddSignalR();

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

// 1️⃣ Регистрируем EventBus
builder.Services.AddSingleton<IEventBus>(sp =>
{
	var config = sp.GetRequiredService<IConfiguration>();
	var connStr = config.GetConnectionString("RabbitMQ")
				  ?? "amqp://guest:guest@localhost:5672/";
	return new RabbitMqEventBus(connStr);
});

builder.Services.AddSingleton<IActiveCompaniesService, ActiveCompaniesService>();
builder.Services.AddHostedService<ReducePriceWorker>();
builder.Services.AddHostedService<ChangePriceService>();
builder.Services.AddTransient<IIncreasePriceService, IncreasePriceService>();   //todo: change

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUserService, UserService>();

//builder.Services.AddQuartz(q => q.AddJobAndTrigger<ReducePriceJob>(builder.Configuration));
//builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	app.UseDeveloperExceptionPage();

	app.UseSwagger();
	app.UseSwaggerUI();

	//app.ApplyMigrations();
	using (var scope = app.Services.CreateScope())
	{
		var services = scope.ServiceProvider;

		var dynamicPriceDb = services.GetRequiredService<DynamicPriceCoreContext>();
		dynamicPriceDb.Database.Migrate();

		var identityDb = services.GetRequiredService<IdentityContext>();
		identityDb.Database.Migrate();

		await DbInitializer.SeedDataAsync(services);
	}

}

app.UseHttpsRedirection();

//todo: cookie - set expired
app.UseCookiePolicy(new CookiePolicyOptions
{
	MinimumSameSitePolicy = SameSiteMode.Strict,
	HttpOnly = HttpOnlyPolicy.Always,
	Secure = CookieSecurePolicy.Always
});

app.UseAuthentication();
app.UseAuthorization();
app.UseCors("AllowSpecificOrigins");

app.MapMetrics();
app.UseHttpMetrics();
app.MapControllers();
app.MapHub<PriceHub>("/priceHub");

app.Run();

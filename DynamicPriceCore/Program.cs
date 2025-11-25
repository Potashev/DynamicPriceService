using DynamicPrice.Core.Services;
using DynamicPriceCore.Data;
using DynamicPriceCore.Extensions;
using DynamicPriceCore.Models;
using DynamicPriceCore.Services;
using DynamicPriceCore.ViewModels;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Prometheus;
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

builder.Services.AddSwaggerGen();

builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<MappingProfile>();
});

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<Program>());

builder.Services.AddSingleton<IEventBus>(sp =>
{
	var config = sp.GetRequiredService<IConfiguration>();
	var connStr = config.GetConnectionString("RabbitMQ")
				  ?? "amqp://guest:guest@localhost:5672/";
	return new RabbitMqEventBus(connStr);
});

builder.Services.AddMassTransit(x =>
{
	x.AddConsumer<ActiveCompaniesService>();
	x.AddConsumer<ChangePriceService>();

	x.UsingInMemory((context, cfg) =>
	{
		cfg.ConfigureEndpoints(context);
	});
});

builder.Services.AddSingleton<ActiveCompaniesService>();
builder.Services.AddHostedService<ReducePriceWorker>();
builder.Services.AddTransient<IIncreasePriceService, IncreasePriceService>();   //todo: change

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUserService, UserService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	app.UseDeveloperExceptionPage();

	app.UseSwagger();
	app.UseSwaggerUI();

	app.ApplyMigrations();
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

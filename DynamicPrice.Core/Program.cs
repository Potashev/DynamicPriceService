using DynamicPrice.Core.Data;
using DynamicPrice.Core.Extensions;
using DynamicPrice.Core.Mapping;
using DynamicPrice.Core.Models;
using DynamicPrice.Core.Services;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Prometheus;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

builder.Services.AddDbContext<DynamicPriceCoreContext>(options =>
	options.UseSqlServer(configuration.GetConnectionString("DynamicPriceDb") ?? throw new InvalidOperationException("Connection string 'DynamicPriceDb' not found.")));
builder.Services.AddDbContext<IdentityContext>(options =>
	options.UseSqlServer(configuration.GetConnectionString("IdentityDb") ?? throw new InvalidOperationException("Connection string 'IdentityDb' not found.")));

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
		IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"])),
		ValidateIssuer = true,
		ValidateAudience = true,
		ValidIssuer = configuration["Jwt:Issuer"] ?? "TestIssuer",
		ValidAudience = configuration["Jwt:Audience"] ?? "TestAudience",
		ValidateLifetime = true,
		ClockSkew = TimeSpan.Zero
	};

	options.Events = new JwtBearerEvents
	{
		OnMessageReceived = context =>
		{
			var accessToken = context.Request.Cookies["DpAuth"];
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
			policy.SetIsOriginAllowedToAllowWildcardSubdomains();
			policy.WithOrigins("https://localhost:7022", "https://localhost:7183")
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

builder.Services.AddMassTransit(x =>
{
	x.AddConsumer<ReducePriceService>();
	x.AddConsumer<IncreasePriceService>();

	x.UsingRabbitMq((context, cfg) =>
	{
		var host = configuration["RabbitMq:Host"] ?? "localhost";
		var user = configuration["RabbitMq:Username"] ?? "guest";
		var pass = configuration["RabbitMq:Password"] ?? "guest";

		cfg.Host(host, h =>
		{
			h.Username(user);
			h.Password(pass);
		});

		cfg.ConfigureEndpoints(context);
	});

	//x.UsingInMemory((context, cfg) =>
	//{
	//	cfg.ConfigureEndpoints(context);
	//});
});

builder.Services.AddHostedService<FindProductsToReduceService>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUserService, UserService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	app.UseDeveloperExceptionPage();

	app.UseSwagger();
	app.UseSwaggerUI();

	await app.ApplyMigrationsAsync();
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

using DynamicPrice.Core.Data;
using DynamicPrice.Core.Extensions;
using DynamicPrice.Core.Mapping;
using DynamicPrice.Core.Middlewares;
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

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
	.AddEntityFrameworkStores<IdentityContext>();

builder.Services
	.AddAuthentication(options =>
	{
		options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
		options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
	})
	.AddJwtBearer(options =>
	{
		options.TokenValidationParameters = new TokenValidationParameters
		{
			ValidateIssuer = true,
			ValidateAudience = true,
			ValidateLifetime = true,
			ValidateIssuerSigningKey = true,
			ValidIssuer = configuration["Jwt:Issuer"],
			ValidAudience = configuration["Jwt:Audience"],
			IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!)),
		};
	});

builder.Services.AddAuthorization(options =>
{
	options.AddPolicy("ManagerPolicy", policy =>
		policy.RequireRole("Manager"));

	options.AddPolicy("CustomerPolicy", policy =>
		policy.RequireRole("Customer"));
});

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

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

//todo: check need it?
app.UseCookiePolicy(new CookiePolicyOptions
{
	MinimumSameSitePolicy = SameSiteMode.Strict,
	HttpOnly = HttpOnlyPolicy.Always,
	Secure = CookieSecurePolicy.Always,
});


app.UseCors("AllowSpecificOrigins");

app.UseAuthentication();
app.UseAuthorization();

app.UseHttpMetrics();

app.MapEndPoints();

app.MapMetrics();
app.MapHub<PriceHub>("/priceHub");

app.Run();

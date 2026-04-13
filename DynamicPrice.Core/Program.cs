using DynamicPrice.Core.Data;
using DynamicPrice.Core.Exceptions;
using DynamicPrice.Core.Extensions;
using DynamicPrice.Core.Mapping;
using DynamicPrice.Core.Models;
using DynamicPrice.Core.Services;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Prometheus;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

builder.Services.AddProblemDetails(configure =>
{
	configure.CustomizeProblemDetails = context =>
	{
		context.ProblemDetails.Extensions.TryAdd("requestId", context.HttpContext.TraceIdentifier);
	};
});
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

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

builder.Services.AddAuthorizationBuilder()
	.AddPolicy("ManagerPolicy", policy =>
		policy.RequireRole("Manager"))
	.AddPolicy("CustomerPolicy", policy =>
		policy.RequireRole("Customer"));

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
	cfg.AddProfile<MappingProfile>());

builder.Services.AddMediatR(cfg =>
	cfg.RegisterServicesFromAssemblyContaining<Program>());

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
});

builder.Services.AddHostedService<FindProductsToReduceService>();

builder.Services.AddHostedService<VirtualCustomersService>();
builder.Services.AddSingleton<VirtualCustomerProvider>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserCompanyService, UserCompanyService>();
builder.Services.AddScoped<ITokenService, JWTTokenService>();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
	app.UseDeveloperExceptionPage();

	app.UseSwagger();
	app.UseSwaggerUI();

	await app.ApplyMigrationsAsync();
}

app.UseExceptionHandler();

app.UseHttpsRedirection();

app.UseCors("AllowSpecificOrigins");

app.UseAuthentication();
app.UseAuthorization();

app.UseHttpMetrics();

app.MapEndPoints();
app.MapHub<PriceHub>("/priceHub");
app.MapMetrics();

app.Run();

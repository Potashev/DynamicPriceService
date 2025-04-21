using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
Console.WriteLine("🚀 Перед AddAuthentication()...");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
	.AddJwtBearer(options =>
	{
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

		//options.TokenValidationParameters = new TokenValidationParameters
		//{
		//	ValidateIssuerSigningKey = true,
		//	IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
		//	ValidateIssuer = true,
		//	ValidateAudience = true,
		//	ValidIssuer = builder.Configuration["Jwt:Issuer"],
		//	ValidAudience = builder.Configuration["Jwt:Audience"],
		//	ValidateLifetime = true,
		//	ClockSkew = TimeSpan.Zero
		//};
	});

Console.WriteLine("🚀 После AddAuthentication()...");


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//app.UseAuthentication();  // Обязательно перед Authorization!
app.UseAuthorization();

app.MapControllers();

app.Run();

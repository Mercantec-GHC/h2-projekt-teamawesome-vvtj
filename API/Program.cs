using API.Data;
using API.Interfaces;
using API.Services;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureKeyVault;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;
using System.Reflection;
using System.Text;

namespace API;

public class Program
{
	public static void Main(string[] args)
	{
		var builder = WebApplication.CreateBuilder(args);
		builder.Services.AddControllers();
		builder.Services.AddOpenApi();

		//Add Interfaces and Services
		builder.Services.AddScoped<IUserService, UserService>();
		builder.Services.AddScoped<IRoleService, RoleService>();
		builder.Services.AddScoped<RoomService>();
		builder.Services.AddScoped<RoomTypeService>();
		builder.Services.AddScoped<ICleaningService, CleaningService>();
		builder.Services.AddScoped<IAuthService, AuthService>();
		builder.Services.AddScoped<HotelService>();
		builder.Services.AddScoped<IBookingInterface, BookingService>();
        builder.Services.AddScoped<ILoginAttemptService, LoginAttemptService>();
		builder.Services.AddScoped<IUserInfoService, UserInfoService>();
		builder.Services.AddMemoryCache();

        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddSwaggerGen(c =>
		{
			var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
			var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
			if (File.Exists(xmlPath))
			{
				c.IncludeXmlComments(xmlPath);
			}

			//Jwt security support for Swagger
			c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
			{
				Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below.",
				Name = "Authorization",
				In = ParameterLocation.Header,
				Type = SecuritySchemeType.ApiKey,
				Scheme = "Bearer"
			});

			c.AddSecurityRequirement(new OpenApiSecurityRequirement()
			{
				{
					new OpenApiSecurityScheme
					{
						Reference = new OpenApiReference
						{
							Type = ReferenceType.SecurityScheme,
							Id = "Bearer"
						},
						Scheme = "oauth2",
						Name = "Bearer",
						In = ParameterLocation.Header,
					},
					new List<string>()
				}
			});
		});

		// Tilføj CORS for specifikke Blazor WASM domæner
		builder.Services.AddCors(options =>
		{
			options.AddPolicy(
				"AllowSpecificOrigins",
				builder =>
				{
					builder
						.WithOrigins(
							"http://localhost:5085",
							"http://localhost:8052",
							"https://h2.mercantec.tech"
						)
						.AllowAnyMethod()
						.AllowAnyHeader()
						.WithExposedHeaders("Content-Disposition");
				}
			);
		});

		// Tilføj basic health checks
		builder.Services.AddHealthChecks()
			.AddCheck("self", () => Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy(), ["live"]);

		if (builder.Environment.IsDevelopment())
		{
			IConfiguration Configuration = builder.Configuration;
			string connectionString = Configuration.GetConnectionString("DefaultConnection")
				?? Environment.GetEnvironmentVariable("DefaultConnection");

			builder.Services.AddDbContext<AppDBContext>(options =>
					options.UseNpgsql(connectionString));

		}

		if (builder.Environment.IsProduction())
		{
			var keyVaultURL = builder.Configuration.GetSection("KeyVault:KeyVaultURL");
			var KeyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(new AzureServiceTokenProvider().KeyVaultTokenCallback));
				builder.Configuration.AddAzureKeyVault(
				keyVaultURL.Value!.ToString(),
				new DefaultKeyVaultSecretManager()
			); 

			var client = new SecretClient(new Uri(keyVaultURL.Value!.ToString()), new DefaultAzureCredential());

            builder.Services.AddDbContext<AppDBContext>(options =>
                    options.UseNpgsql(client.GetSecret("dbconnection").Value.Value.ToString()));
        }

		builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
			.AddJwtBearer(options =>
			{
				options.TokenValidationParameters = new TokenValidationParameters
				{
					ValidateIssuer = true,
					ValidateAudience = true,
					ValidIssuer = builder.Configuration["AppSettings:Issuer"],
					ValidAudience = builder.Configuration["AppSettings:Audience"],
					ValidateLifetime = true,
					IssuerSigningKey = new SymmetricSecurityKey(
						Encoding.UTF8.GetBytes(builder.Configuration["AppSettings:Token"]!)),
					ValidateIssuerSigningKey = true,
				};
			})
			;

		var app = builder.Build();

		// Brug CORS - skal være før anden middleware
		app.UseCors("AllowSpecificOrigins");

		// Map health checks
		app.MapHealthChecks("/health");
		app.MapHealthChecks("/alive", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
		{
			Predicate = r => r.Tags.Contains("live")
		});


		if (app.Environment.IsDevelopment())

		{
			app.MapOpenApi();
			app.MapScalarApiReference();
		}

		// Scalar Middleware for OpenAPI
		//app.MapScalarApiReference(options =>
		//{
		//    options
		//        .WithTitle("MAGSLearn")
		//        .WithTheme(ScalarTheme.Mars)
		//        .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
		//});

		// Map the Swagger UI
		app.UseSwagger();
		app.UseSwaggerUI(options =>
		{
			options.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1");
		});

		app.UseAuthentication();
		app.UseAuthorization();

		app.MapControllers();

		app.Run();
	}
}

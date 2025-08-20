using API.Data;
using API.Interfaces;
using API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
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
        builder.Services.AddScoped<IBookingService, BookingService>();

        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddSwaggerGen(c =>
        {
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
            {
                c.IncludeXmlComments(xmlPath);
            }
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

        IConfiguration Configuration = builder.Configuration;
        string connectionString = Configuration.GetConnectionString("DefaultConnection")
            ?? Environment.GetEnvironmentVariable("DefaultConnection");

        builder.Services.AddDbContext<AppDBContext>(options =>
                options.UseNpgsql(connectionString));

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

        
        if(app.Environment.IsDevelopment())

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

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}

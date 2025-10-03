using Blazor.Services;
using BlazorBootstrap;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.Authorization;
using Blazored.LocalStorage;
using Blazored.SessionStorage;
using Blazor.Interfaces;
namespace Blazor;

public class Program
{
	public static async Task Main(string[] args)
	{
		var builder = WebAssemblyHostBuilder.CreateDefault(args);
		builder.RootComponents.Add<App>("#app");
		builder.RootComponents.Add<HeadOutlet>("head::after");

		builder.Services.AddScoped<ToastService>();
		builder.Services.AddScoped<CustomAuthStateProvider>();
		builder.Services.AddScoped<AuthenticationStateProvider>(provider =>
			provider.GetRequiredService<CustomAuthStateProvider>());
		builder.Services.AddScoped<IAuthService, AuthService>();
		builder.Services.AddScoped<PreloadService>();
		builder.Services.AddAuthorizationCore();
		builder.Services.AddBlazoredLocalStorage();
		builder.Services.AddBlazoredSessionStorage();
		builder.Services.AddCascadingAuthenticationState();
		builder.Services.AddScoped<AuthMessageHandler>();

		var env = builder.HostEnvironment.Environment;
		string apiEndpoint;

		if (env == "Development")
		{
			apiEndpoint = "https://localhost:8000/";
		}
		else
		{
			apiEndpoint = Environment.GetEnvironmentVariable("API_ENDPOINT")
						  ?? "https://prod-novahotels-api-mercantec-tech.azurewebsites.net/";
		}

		// Register HttpClient for APIService with AuthMessageHandler
		builder.Services.AddHttpClient<APIService>(client =>
		{
			client.BaseAddress = new Uri(apiEndpoint);
			Console.WriteLine($"APIService BaseAddress: {client.BaseAddress}");
		})
		.AddHttpMessageHandler<AuthMessageHandler>();

		// Register a basic HttpClient for TokenService without AuthMessageHandler
		builder.Services.AddScoped(sp => new HttpClient
		{
			BaseAddress = new Uri(apiEndpoint)
		});
		builder.Services.AddScoped<ITokenService, TokenService>();

		await builder.Build().RunAsync();
	}
}

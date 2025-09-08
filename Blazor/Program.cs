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

		builder.Services.AddAuthorizationCore();
		builder.Services.AddBlazoredLocalStorage();
        builder.Services.AddBlazoredSessionStorage();
		builder.Services.AddCascadingAuthenticationState();

		// Læs API endpoint fra miljøvariabler eller brug default
		var envApiEndpoint = Environment.GetEnvironmentVariable("API_ENDPOINT");
       
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

		// Registrer HttpClient til API service med konfigurerbar endpoint

		//This configuration creates new HttpClient every time the DI resolves APIService
		//I am using DefaultRequestHeaders.Authorization to SetBearerToken(), so the user stays authorized and authenticated accross the refreshes
		//EVery time a new HttpClient is created, bearer information is being removed from the header and Blazor no longer knows that the user is authorized.


		//builder.Services.AddHttpClient<APIService>(client =>
		//{
		//    client.BaseAddress = new Uri(apiEndpoint);
		//    Console.WriteLine($"APIService BaseAddress: {client.BaseAddress}");
		//});

		//If possible I would like to keep this configuration. This way same HttpClient reused across the app session.
		//Perfect for Blazor WASM, because under the hood it’s just calling the browser’s fetch() API.
		builder.Services.AddScoped(sp => new HttpClient
		{
			BaseAddress = new Uri(apiEndpoint)
		});
		builder.Services.AddScoped<APIService>();

		await builder.Build().RunAsync();
    }
}

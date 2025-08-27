using System;
using System.Net.Http;
using Blazor.Services;
using BlazorBootstrap;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Blazor;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);
        builder.RootComponents.Add<App>("#app");
        builder.RootComponents.Add<HeadOutlet>("head::after");

		builder.Services.AddScoped<ToastService>();

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
        builder.Services.AddHttpClient<APIService>(client =>
        {
            client.BaseAddress = new Uri(apiEndpoint);
            Console.WriteLine($"APIService BaseAddress: {client.BaseAddress}");
        });

        await builder.Build().RunAsync();
    }
}

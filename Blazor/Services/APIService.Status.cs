using DomainModels;
using System.Net.Http.Json;

namespace Blazor.Services
{
    public partial class APIService
    {
        public async Task<HealthCheckResponse?> GetHealthCheckAsync()
        {
            try
            {
                Console.WriteLine($"status link: {httpClient.BaseAddress}api/Status/healthcheck");
                return await httpClient.GetFromJsonAsync<HealthCheckResponse>("api/Status/healthcheck");
            }
            catch (Exception ex)
            {
                // Her kan du evt. logge fejlen
                Console.WriteLine("Fejl ved HealthCheck: " + ex.Message);
                return new HealthCheckResponse
                {
                    status = "Error",
                    message = "Kunne ikke hente API-status (" + ex.Message + ")"
                };
            }
        }
        public async Task<HealthCheckResponse?> GetDBHealthCheckAsync()
        {
            try
            {
                return await httpClient.GetFromJsonAsync<HealthCheckResponse>("api/Status/dbhealthcheck");
            }
            catch (Exception ex)
            {
                // Her kan du evt. logge fejlen
                Console.WriteLine("Fejl ved DBHealthCheck: " + ex.Message);
                return new HealthCheckResponse
                {
                    status = "Error",
                    message = "Kunne ikke hente database-status (" + ex.Message + ")"
                };
            }
        }
    }
}

namespace Blazor.Services
{
    public partial class APIService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<APIService> _logger;

        public APIService(HttpClient httpClient, ILogger<APIService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }
    }
}

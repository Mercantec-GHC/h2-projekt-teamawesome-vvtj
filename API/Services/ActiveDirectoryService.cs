using System.Net;
using System.DirectoryServices.Protocols;
using System.Text.RegularExpressions;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace API.Services
{
    public partial class ActiveDirectoryService
    {
        private readonly ILogger<ActiveDirectoryService> _logger;

        // AD konfiguration fra appsettings.json
        private readonly string _server;
        private readonly string _username;
        private readonly string _password;
        private readonly string _domain;
        private readonly int _port;
        private readonly bool _useSSL;

        /// <summary>
        /// Initialiserer en ny instans af ActiveDirectoryService
        /// </summary>
        /// <param name="logger">Logger til fejlrapportering</param>
        /// <param name="configuration">Konfiguration til AD indstillinger</param>
        public ActiveDirectoryService(ILogger<ActiveDirectoryService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;

            // Læs AD konfiguration fra appsettings.json
            _server = _configuration["ActiveDirectory:Server"];
            _domain = _configuration["ActiveDirectory:Domain"];
            _username = _configuration["ActiveDirectory:ReaderUsername"];
            _password = _configuration["ActiveDirectory:ReaderPassword"];
            _port = int.Parse(_configuration["ActiveDirectory:Port"]);
            _useSSL = bool.Parse(_configuration["ActiveDirectory:UseSSL"]);

            _configuration = configuration;
            _secretKey = "MyVerySecureSecretKeyThatIsAtLeast32CharactersLong123456789";
            _issuer = _configuration["AppSettings:Issuer"] ?? Environment.GetEnvironmentVariable("JWT_ISSUER") ?? "NOVAHotels";
            _audience = _configuration["AppSettings:Audience"] ?? Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? "NOVAHotelsUsers";

            Console.WriteLine($"Loaded JWT Secret Key: {_secretKey}");
            
        }
        /// <summary>
        /// Autentificerer en bruger mod Active Directory
        /// </summary>
        /// <param name="username">Brugernavn (kan være email eller sAMAccountName)</param>
        /// <param name="password">Adgangskode</param>
        /// <returns>ADUserInfo med brugerinformation hvis autentificering lykkes, ellers null</returns>
        public async Task<ADUserInfo?> AuthenticateUserAsync(string username, string password)
        {

            // Opret LDAP forbindelse
            using var connection = new LdapConnection(new LdapDirectoryIdentifier(_server, _port));
            connection.SessionOptions.ProtocolVersion = 3;
            connection.SessionOptions.SecureSocketLayer = false;
            connection.SessionOptions.VerifyServerCertificate = (conn, cert) => true;

            // Opret credentials for AD reader bruger
            var networkCredential = new NetworkCredential(_username, _password, _domain);
            connection.Credential = networkCredential;

            // Åbn forbindelse
            await Task.Run(() => connection.Bind());

            // Søg efter brugeren i AD
            var userInfo = await SearchUserInADAsync(connection, username);
            if (userInfo == null)
                return null;

            // Test brugerens credentials
            var userCredentials = new NetworkCredential(userInfo.SamAccountName, password, _domain);
            using var userConnection = new LdapConnection(new LdapDirectoryIdentifier(_server, _port));
            userConnection.SessionOptions.ProtocolVersion = 3;
            userConnection.SessionOptions.SecureSocketLayer = false;
            userConnection.SessionOptions.VerifyServerCertificate = (conn, cert) => true;
            userConnection.Credential = userCredentials;

            // Test autentificering
            await Task.Run(() => userConnection.Bind());
           

            return userInfo;
        }
        private async Task<ADUserInfo?> SearchUserInADAsync(LdapConnection connection, string username)
        {
            
                // Konstruer søgning - søg både på sAMAccountName og email
                var searchFilter = $"(|(sAMAccountName={username})(mail={username})(userPrincipalName={username}))";
                var searchRequest = new SearchRequest(
                    $"DC={_domain.Split('.')[0]},DC={_domain.Split('.')[1]}", // Konstruer base DN
                    searchFilter,
                    SearchScope.Subtree,
                    "sAMAccountName", "mail", "displayName", "givenName", "sn", "memberOf", "userPrincipalName"
                );

                var searchResponse = await Task.Run(() => (SearchResponse)connection.SendRequest(searchRequest));

                if (searchResponse.Entries.Count == 0)
                {
                    _logger.LogWarning("Ingen bruger fundet i AD for: {Username}", username);
                    return null;
                }

                var entry = searchResponse.Entries[0];
                
                // Debug: Log alle tilgængelige attributter
                _logger.LogInformation("Tilgængelige attributter for bruger:");
                foreach (string attrName in entry.Attributes.AttributeNames)
                {
                    _logger.LogInformation("Attribut: {AttrName}, Værdier: {Count}", attrName, entry.Attributes[attrName].Count);
                }
                
                var userInfo = new ADUserInfo
                {
                    SamAccountName = GetAttributeValue(entry, "sAMAccountName"),
                    Email = GetAttributeValue(entry, "mail"),
                    
                    FirstName = GetAttributeValue(entry, "givenName"),
                    LastName = GetAttributeValue(entry, "sn"),
                };

                _logger.LogInformation("Bruger fundet i AD: {SamAccountName}, Email: {Email}, Groups: {GroupCount}", 
                    userInfo.SamAccountName, userInfo.Email, userInfo.Groups.Count);

                return userInfo;
            
        }
        private string GetAttributeValue(SearchResultEntry entry, string attributeName)
        {
            if (entry.Attributes[attributeName] != null && entry.Attributes[attributeName].Count > 0)
            {
                return entry.Attributes[attributeName][0].ToString() ?? string.Empty;
            }
            return string.Empty;
        }
        

        //-----------------------------------------------------------------------------//
        // Modeller for AD objekter
        public class ADGroup
        {
            public string Name { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
            public List<string> Members { get; set; } = new List<string>();
        }

        public class ADUser
        {
            public string Name { get; set; } = string.Empty;
            public string Username { get; set; } = string.Empty;
            //   public string? Email { get; set; } = string.Empty;
            public string Department { get; set; } = string.Empty;
            public string Title { get; set; } = string.Empty;
            //   public string DistinguishedName { get; set; } = string.Empty;
            public string FirstName { get; set; } = string.Empty;
            public string LastName { get; set; } = string.Empty;
            //     public string DisplayName { get; set; } = string.Empty;
            public string Company { get; set; } = string.Empty;
            public string Office { get; set; } = string.Empty;
            //   public string Phone { get; set; } = string.Empty;
            //  public string Mobile { get; set; } = string.Empty;
            public string Manager { get; set; } = string.Empty;
            //public DateTime? LastLogon { get; set; }
            //public DateTime? PasswordLastSet { get; set; }
            //   public bool IsEnabled { get; set; } = true;
            public List<string> Groups { get; set; } = new List<string>();
        }

        // Konfigurationsklasse for runtime indstillinger
        public class ADConfig
        {
            public string Server { get; set; } = string.Empty;
            public string Username { get; set; } = string.Empty;
            public string? Password { get; set; } = string.Empty;
            public string Domain { get; set; } = string.Empty;
        }

        private ADConfig _config;

        public ActiveDirectoryService()
        {
            _config = new ADConfig();
        }

        public ActiveDirectoryService(ADConfig config)
        {
            _config = config;
        }

        // Public properties for at få adgang til konfiguration
        public ADConfig Config => _config;
        public string Server => _config.Server;
        public string Username => _config.Username;
        public string Domain => _config.Domain;

        // Metode til at opdatere konfiguration
        public void UpdateConfig(ADConfig newConfig)
        {
            _config = newConfig;
        }

        // Metode til at opdatere individuelle konfigurationsværdier
        public void UpdateConfig(string? server = null, string? username = null, string? password = null, string? domain = null)
        {
            if (!string.IsNullOrEmpty(server))
                _config.Server = server;
            if (!string.IsNullOrEmpty(username))
                _config.Username = username;
            if (!string.IsNullOrEmpty(password))
                _config.Password = password;
            if (!string.IsNullOrEmpty(domain))
                _config.Domain = domain;
        }

        public class ADUserInfo
        {
            /// <summary>
            /// SAM Account Name fra Active Directory
            /// </summary>
            public string SamAccountName { get; set; } = string.Empty;

            /// <summary>
            /// Email adresse fra Active Directory
            /// </summary>
            public string Email { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;

            /// <summary>
            /// Fornavn fra Active Directory
            /// </summary>
            public string FirstName { get; set; } = string.Empty;

            /// <summary>
            /// Efternavn fra Active Directory
            /// </summary>
            public string LastName { get; set; } = string.Empty;

            public string Department { get; set; } = string.Empty;

            /// <summary>
            /// Liste af grupper brugeren er medlem af i Active Directory
            /// </summary>
            public List<string> Groups { get; set; } = new List<string>();
        }
    }
}

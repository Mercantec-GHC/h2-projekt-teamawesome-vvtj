using System.Net;
using System.DirectoryServices.Protocols;
using DomainModels.Enums;
using System.Text.RegularExpressions;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Crypto.Prng;
using DomainModels.Models;

namespace API.Services
{
    public partial class ActiveDirectoryService
    {
        private readonly ILogger<ActiveDirectoryService> _logger;

        // AD configuration from appsettings.json
        private readonly string _server;
        private readonly string _username;
        private readonly string _password;
        private readonly string _domain;
        private readonly int _port;

        /// <summary>
        /// Initializes a new instance of the ActiveDirectoryService.
        /// </summary>
        /// <param name="logger">Logger used for error reporting</param>
        /// <param name="configuration">Configuration for AD settings</param>
        public ActiveDirectoryService(ILogger<ActiveDirectoryService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;

            // Reads AD configuration from appsettings.json
            _server = _configuration["ActiveDirectory:Server"];
            _domain = _configuration["ActiveDirectory:Domain"];
            _username = _configuration["ActiveDirectory:ReaderUsername"];
            _password = _configuration["ActiveDirectory:ReaderPassword"];
            _port = int.Parse(_configuration["ActiveDirectory:Port"]);

            _configuration = configuration;
            _secretKey = "MyVerySecureSecretKeyThatIsAtLeast32CharactersLong123456789";
            _issuer = _configuration["AppSettings:Issuer"] ?? Environment.GetEnvironmentVariable("JWT_ISSUER") ?? "NOVAHotels";
            _audience = _configuration["AppSettings:Audience"] ?? Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? "NOVAHotelsUsers";

            Console.WriteLine($"Loaded JWT Secret Key: {_secretKey}");
            
        }
        /// <summary>
        /// Authenticates a user via the Active Directory
        /// </summary>
        /// <param name="username">Username (can be email, sAMAccountName, or userPrincipalName)</param>
        /// <param name="password">Password</param>
        /// <returns>ADUserInfo containing user details if authentication is successful; otherwise, null</returns>
        public async Task<ADUserInfo?> AuthenticateUserAsync(string username, string password)
        {

            // Create LDAP connection
            using var connection = new LdapConnection(new LdapDirectoryIdentifier(_server, _port));
            connection.SessionOptions.ProtocolVersion = 3;
            connection.SessionOptions.SecureSocketLayer = false;
            connection.SessionOptions.VerifyServerCertificate = (conn, cert) => true;

            // Create credentials for AD reader user
            var networkCredential = new NetworkCredential(_username, _password, _domain);
            connection.Credential = networkCredential;

            // Open the connection
            await Task.Run(() => connection.Bind());

            // Search for the user in the AD
            var userInfo = await SearchUserInADAsync(connection, username);
            if (userInfo == null)
                return null;

            userInfo.Role = MapADGroupToRole(userInfo.Groups);

            // Test users credentials
            var userCredentials = new NetworkCredential(userInfo.SamAccountName, password, _domain);
            using var userConnection = new LdapConnection(new LdapDirectoryIdentifier(_server, _port));
            userConnection.SessionOptions.ProtocolVersion = 3;
            userConnection.SessionOptions.SecureSocketLayer = false;
            userConnection.SessionOptions.VerifyServerCertificate = (conn, cert) => true;
            userConnection.Credential = userCredentials;

            // Test authentification
            await Task.Run(() => userConnection.Bind());
           
            return userInfo;
        }
        
        /// <summary>
        /// Searches for a user in Active Directory using sAMAccountName, email, or userPrincipalName
        /// </summary>
        /// <param name="connection">An active LDAP connection</param>
        /// <param name="username">The username to search for</param>
        /// <returns>ADUserInfo with user attributes if found; otherwise, null</returns>
        private async Task<ADUserInfo?> SearchUserInADAsync(LdapConnection connection, string username)
        {
            //Finds user whose name match the input "username"
            var searchFilter = $"(|(sAMAccountName={username})(mail={username})(userPrincipalName={username}))";

            //constructs a search request with base DN (domain) and desired attributes
            var searchRequest = new SearchRequest(
                $"DC={_domain.Split('.')[0]},DC={_domain.Split('.')[1]}", // Constructs base DN - Tells LDAP where to search
                searchFilter,                                             // Filter for user
                SearchScope.Subtree,                                      // Searches the entire directory tree with the given attributes
                "sAMAccountName", "mail", "givenName", "sn", "memberOf", "userPrincipalName", "title", "department", "distinguishedName"
            );

            //Executes the search asynchronously
            var searchResponse = await Task.Run(() => (SearchResponse)connection.SendRequest(searchRequest));
            if (searchResponse.Entries.Count == 0)
            {
                _logger.LogWarning("Ingen bruger fundet i AD for: {Username}", username);
                return null;
            }
            var entry = searchResponse.Entries[0];

            // Debugging purposes: Log all available attributtes
            _logger.LogInformation("Tilgængelige attributter for bruger:");
            foreach (string attrName in entry.Attributes.AttributeNames)
            {
                _logger.LogInformation("Attribut: {AttrName}, Værdier: {Count}", attrName, entry.Attributes[attrName].Count);
            }

            //Map attributes to the ADUserInfo model
            var userInfo = new ADUserInfo
            {
                SamAccountName = GetAttributeValue(entry, "sAMAccountName").ToString(),
                Email = GetAttributeValue(entry, "mail").ToString(),
                FirstName = GetAttributeValue(entry, "givenName").ToString(),
                LastName = GetAttributeValue(entry, "sn").ToString(),
                Title = GetAttributeValue(entry, "title").ToString(),
                Department = GetAttributeValue(entry, "department").ToString(),
                MemberOf = GetAttributeValue(entry, "memberOf"),
                DistinguishedName = GetAttributeValue(entry, "distinguishedName").ToString()
            };
            userInfo.Groups = GetGroupsByUser(connection, userInfo.DistinguishedName);

            _logger.LogInformation("Bruger fundet i AD: {SamAccountName}, Email: {Email}, Groups: {GroupCount}",
                userInfo.SamAccountName, userInfo.Email, userInfo.Groups.Count);

            return userInfo;
        }
        private List<string> GetAttributeValue(SearchResultEntry entry, string attributeName)
        {
            List<string> values = new List<string>();
            //Checks if an attribute exists and has at least one value
            if (entry.Attributes[attributeName] != null && entry.Attributes[attributeName].Count > 0)
            {
                foreach (var val in entry.Attributes[attributeName])
                {
                    values.Add(val.ToString());
                }
            }
            //If not found or emoty, return empty string
            return values;
        }

        private List<string> GetGroupsByUser(LdapConnection connection, string? userDN)
        {
            List<string> Groups = new List<string>();
            var groupSearch = new SearchRequest(
                    $"DC={_domain.Split('.')[0]},DC={_domain.Split('.')[1]}",
                    $"(member={userDN})",
                    SearchScope.Subtree,
                    "cn", "description"
                );
            try
            {
                var groupResponse = (SearchResponse)connection.SendRequest(groupSearch);
                if (groupResponse.Entries.Count == 0)
                    return Groups;

                foreach (SearchResultEntry entry in groupResponse.Entries)
                {
                    if (entry.Attributes.Contains("cn"))
                    {
                        var groupName = entry.Attributes["cn"][0]?.ToString();
                        if (!string.IsNullOrEmpty(groupName))
                        {
                            Groups.Add(groupName);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fejl ved hentning af grupper for {userDN}: {ex.Message}");
            }
            return Groups;
        }

        private RoleEnum MapADGroupToRole(List<string> adGroups)
        {
            //Map AD groups to our RoleEnum
            Dictionary<string, RoleEnum> GroupToRoleMap = new(StringComparer.OrdinalIgnoreCase) //Case sensitive
            {
                {"Admin", RoleEnum.Admin},
                {"receptionist", RoleEnum.Reception},
                {"cleaningStaff", RoleEnum.CleaningStaff}
            };

            return adGroups
            .Select(group => GroupToRoleMap.GetValueOrDefault(group, RoleEnum.Unknown))
            .FirstOrDefault(role => role != RoleEnum.Unknown);

            // if (GroupToRoleMap.TryGetValue(group, out var role))
            // {
            //     return role;
            // }
        }


        //-----------------------------------------------------------------------------//
        // Models for AD objects
        public class ADGroup
        {
            public string Name { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
            public List<string> Members { get; set; } = new List<string>();
        }

        public class ADUser
        {
            //Variables have been commented out, because we dont need them as of right now
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
            //public bool IsEnabled { get; set; } = true;
            public List<string> Groups { get; set; } = new List<string>();
        }
       

        //Model for the authenticated AD user
        public class ADUserInfo
        {
            public string? SamAccountName { get; set; } = string.Empty;
            public string? Email { get; set; } = string.Empty;
            public string? Password { get; set; } = string.Empty;
            public string? FirstName { get; set; } = string.Empty;
            public string? LastName { get; set; } = string.Empty;
            public string? Department { get; set; } = string.Empty;
            public string? Title { get; set; } = string.Empty;
            public List<string> MemberOf { get; set; } = new List<string>();
            public string? DistinguishedName { get; set; } = string.Empty;
            public RoleEnum Role { get; set; } = RoleEnum.Unknown;
            
            //List of AD groups the user belongs to
            public List<string> Groups { get; set; } = new List<string>();
        }
    }
}

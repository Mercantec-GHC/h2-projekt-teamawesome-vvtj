using System.Net;
using System.DirectoryServices.Protocols;
using API.Interfaces;
using Microsoft.CodeAnalysis.Elfie.Serialization;
using API.Data;
using Microsoft.EntityFrameworkCore;
using DomainModels.Models;
using DomainModels.Dto.UserDto;
using DomainModels.Enums;

namespace API.Services
{
	public partial class ActiveDirectoryService
	{
		private readonly ILogger<ActiveDirectoryService> _logger;
		private readonly IConfiguration _configuration;
		private readonly IJWTService _jwtService;
		private readonly AppDBContext _context;

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
		public ActiveDirectoryService(ILogger<ActiveDirectoryService> logger, IConfiguration configuration, IJWTService jwtService, AppDBContext context)
		{
			_logger = logger;
			_configuration = configuration;
			_jwtService = jwtService;
			_context = context;

			// Reads AD configuration from appsettings.json
			_server = _configuration["ActiveDirectory:Server"]!;
			_domain = _configuration["ActiveDirectory:Domain"]!;
			_username = _configuration["ActiveDirectory:ReaderUsername"]!;
			_password = _configuration["ActiveDirectory:ReaderPassword"]!;
			_port = int.Parse(_configuration["ActiveDirectory:Port"]!);
		}

		/// <summary>
		/// Attempts to authenticate an Active Directory user and returns a JWT token if successful.
		/// </summary>
		/// <param name="username">The username of the AD user (can be email, sAMAccountName, or userPrincipalName).</param>
		/// <param name="password">The password of the AD user.</param>
		/// <returns>
		/// A JWT token string if authentication is successful; otherwise, <c>null</c>.
		/// </returns>
		public async Task<string?> LoginADUserAsync(string username, string password)
		{
			try
			{
				// Authenticate the user against Active Directory
				var adUser = await AuthenticateUserAsync(username, password);
				if (adUser == null)
					return null;

				var roleEntity = await _context.Roles
				.FirstOrDefaultAsync(r => r.RoleName == adUser.Role)
					?? throw new ArgumentException($"Role '{adUser.Role}' was not found in the database");

				var adUserToDb = await _context.Users
				.Include(u => u.UserRole)
				.FirstOrDefaultAsync(u => u.UserName == adUser.SamAccountName);
				if (adUserToDb == null)
				{
					adUserToDb = new User
					{
						UserName = adUser.SamAccountName,
						Email = adUser.Email,
						HashedPassword = "EXTERNALLY_MANAGED",
						UserRole = roleEntity,
					};
					_context.Users.Add(adUserToDb);
					await _context.SaveChangesAsync();
				}

				_logger.LogInformation("Raw AD department value: '{Department}'", adUser.Department);

				adUser.Department = await MapADUserDepartmentToHotelName(adUser.Department)
				?? throw new ArgumentException($"Department '{adUser.Department}' does not match any known hotel in the system.");

				// Generate a JWT token for the authenticated AD user
				var token = _jwtService.CreateToken(adUser);

				return token;
			}
			catch (Exception)
			{
				throw;
			}

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
				"sAMAccountName", "mail", "displayName", "givenName", "sn", "memberOf", "userPrincipalName", "distinguishedName", "department"
			);

			//Executes the search asynchronously
			var searchResponse = await Task.Run(() => (SearchResponse)connection.SendRequest(searchRequest));

			if (searchResponse.Entries.Count == 0)
			{
				_logger.LogWarning($"No users found in the AD with the name: {username}");
				return null;
			}

			var entry = searchResponse.Entries[0];

			// Debugging purposes: Log all available attributtes
			_logger.LogInformation("Available attribute for users:");
			foreach (string attrName in entry.Attributes.AttributeNames)
			{
				_logger.LogInformation("Attribute: {AttrName}, Value: {Count}", attrName, entry.Attributes[attrName].Count);
			}

			//Map attributes to the ADUserInfo model
			var userInfo = new ADUserInfo
			{
				SamAccountName = GetAttributeValue(entry, "sAMAccountName"),
				Email = GetAttributeValue(entry, "mail"),
				FirstName = GetAttributeValue(entry, "givenName"),
				LastName = GetAttributeValue(entry, "sn"),
				DistinguishedName = GetAttributeValue(entry, "distinguishedName"),
				Department = GetAttributeValue(entry, "department")
			};

			userInfo.Groups = GetGroupsByUser(connection, userInfo.DistinguishedName);
			userInfo.Role = MapADGroupToRole(userInfo.Groups);

			_logger.LogInformation("User found in AD: {SamAccountName}, Email: {Email}, Groups: {GroupCount}",
				userInfo.SamAccountName, userInfo.Email, userInfo.Groups.Count);

			return userInfo;

		}
		private string GetAttributeValue(SearchResultEntry entry, string attributeName)
		{
			//Checks if an attribute exists and has at least one value
			if (entry.Attributes[attributeName] != null && entry.Attributes[attributeName].Count > 0)
			{
				return entry.Attributes[attributeName][0].ToString() ?? string.Empty;
			}
			//If not found or emoty, return empty string
			return string.Empty;
		}

		private List<string> GetGroupsByUser(LdapConnection connection, string? userDN)
        {
            List<string> groups = new List<string>();
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
					return groups;
				foreach (SearchResultEntry entry in groupResponse.Entries)
				{
					var name = entry.Attributes["cn"]?[0]?.ToString() ?? string.Empty;
					var desc = entry.Attributes["description"]?[0]?.ToString() ?? string.Empty;
					if (!string.IsNullOrEmpty(name))
					{
						groups.Add(name);
						groups.Add(desc);
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Could not find groups for {userDN}: {ex.Message}");
			}
			return groups;
		}

		private RoleEnum MapADGroupToRole(List<string> adGroups)
		{
			//Map AD groups to our RoleEnum
			Dictionary<string, RoleEnum> GroupToRoleMap = new(StringComparer.OrdinalIgnoreCase) //Case sensitive
            {
				{"Admin", RoleEnum.Admin},
				{"Reception", RoleEnum.Reception},
				{"CleaningStaff", RoleEnum.CleaningStaff}
			};

			foreach (var description in adGroups)
			{
				if (GroupToRoleMap.TryGetValue(description, out var role))
					return role;
			}

			return RoleEnum.Unknown;
		}

		private async Task<string?> MapADUserDepartmentToHotelName(string department)
		{
			if (string.IsNullOrWhiteSpace(department))
				return null;

			var normalizedDept = department.Trim().ToLower();

			var hotel = await _context.Hotels
				.FirstOrDefaultAsync(h => h.HotelName.ToLower() == normalizedDept.ToLower());

			return hotel?.HotelName;
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
			public string SamAccountName { get; set; } = string.Empty;
			public string Email { get; set; } = string.Empty;
			public string Password { get; set; } = string.Empty;
			public string FirstName { get; set; } = string.Empty;
			public string LastName { get; set; } = string.Empty;
			public string Department { get; set; } = string.Empty;
			public string DistinguishedName { get; set; } = string.Empty;
			public RoleEnum Role { get; set; } = RoleEnum.Unknown;
			//List of AD groups the user belongs to
			public List<string> Groups { get; set; } = new List<string>();
		}
	}
}
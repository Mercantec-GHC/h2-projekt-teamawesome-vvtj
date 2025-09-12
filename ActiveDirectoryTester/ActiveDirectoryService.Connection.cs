using System.DirectoryServices.Protocols;
using System.Net;

namespace ActiveDirectoryTesting
{
    public partial class ActiveDirectoryService
    {
        /// <summary>
        /// Opretter en forbindelse til Active Directory
        /// </summary>
        /// <returns>LdapConnection objekt</returns>
        public LdapConnection GetConnection()
        {
            var server = _config.Server = "10.133.71.114";
            var credential = new NetworkCredential($"{_config.Username}@{_config.Domain}", _config.Password);
            var connection = new LdapConnection(_config.Server)
            {
                Credential = credential,
                AuthType = AuthType.Negotiate
            };

            connection.Bind(); // Test forbindelse
            return connection;
        }

        /// <summary>
        /// Tester forbindelsen til Active Directory med detaljeret information
        /// </summary>
        public void TestConnection()
        {
            Console.WriteLine("=== Test Forbindelse til Active Directory ===");
            Console.WriteLine();

            // Vis forbindelsesoplysninger
            Console.WriteLine("Forbindelsesoplysninger:");
            Console.WriteLine($"  Server: {_config.Server}");
            Console.WriteLine($"  Domæne: {_config.Domain}");
            Console.WriteLine($"  Brugernavn: {_config.Username}");
            Console.WriteLine($"  Port: 389 (standard LDAP)");
            Console.WriteLine();

            Console.WriteLine("Forsøger at oprette forbindelse...");

            try
            {
                // Vis hvad der sker
                Console.WriteLine("  → Opretter NetworkCredential...");
                var credential = new NetworkCredential($"{_config.Username}@{_config.Domain}", _config.Password);

                Console.WriteLine("  → Opretter LdapConnection...");
                var connection = new LdapConnection(_config.Server)
                {
                    Credential = credential,
                    AuthType = AuthType.Negotiate
                };

                Console.WriteLine("  → Tester autentificering (Bind)...");
                connection.Bind();

                Console.WriteLine();
                Console.WriteLine("✅ Forbindelse succesfuld!");
                Console.WriteLine($"   Forbundet til: {_config.Server}");
                Console.WriteLine($"   Autentificeret som: {_config.Username}@{_config.Domain}");
                Console.WriteLine($"   Autentificeringstype: {connection.AuthType}");

                // Test en simpel søgning for at verificere at vi kan læse data
                Console.WriteLine();
                Console.WriteLine("Tester dataadgang...");
                var testSearchRequest = new SearchRequest(
                    $"DC={_config.Domain.Split('.')[0]},DC={_config.Domain.Split('.')[1]}",
                    "(objectClass=*)",
                    SearchScope.Base,
                    "objectClass"
                );

                var testResponse = (SearchResponse)connection.SendRequest(testSearchRequest);
                Console.WriteLine("  → Dataadgang verificeret!");
                Console.WriteLine($"   Base DN: {testResponse.Entries[0].DistinguishedName}");

                connection.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine();
                Console.WriteLine("❌ Forbindelse fejlede!");
                Console.WriteLine($"   Fejltype: {ex.GetType().Name}");
                Console.WriteLine($"   Fejlbesked: {ex.Message}");

                // Giv specifikke fejlforslag baseret på fejltypen
                if (ex.Message.Contains("timeout") || ex.Message.Contains("time-out"))
                {
                    Console.WriteLine();
                    Console.WriteLine("💡 Mulige løsninger:");
                    Console.WriteLine("   - Tjek at serveren er tilgængelig");
                    Console.WriteLine("   - Tjek netværksforbindelse");
                    Console.WriteLine("   - Tjek firewall indstillinger");
                }
                else if (ex.Message.Contains("authentication") || ex.Message.Contains("credentials"))
                {
                    Console.WriteLine();
                    Console.WriteLine("💡 Mulige løsninger:");
                    Console.WriteLine("   - Tjek brugernavn og adgangskode");
                    Console.WriteLine("   - Tjek at brugeren har adgang til AD");
                    Console.WriteLine("   - Tjek domænenavn");
                }
                else if (ex.Message.Contains("server") || ex.Message.Contains("host"))
                {
                    Console.WriteLine();
                    Console.WriteLine("💡 Mulige løsninger:");
                    Console.WriteLine("   - Tjek server IP/adresse");
                    Console.WriteLine("   - Tjek at LDAP service kører på serveren");
                    Console.WriteLine("   - Tjek port 389 er åben");
                }
            }
        }

        /// <summary>
        /// Hjælpe metode til at få base DN for domænet
        /// </summary>
        /// <returns>Base DN string</returns>
        private string GetBaseDN()
        {
            return $"DC={_config.Domain.Split('.')[0]},DC={_config.Domain.Split('.')[1]}";
        }
    }
}
 public class ADLoginDto
    {
        /// <summary>
        /// Brugernavn (kan være sAMAccountName, email eller userPrincipalName)
        /// </summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Adgangskode
        /// </summary>
        public string Password { get; set; } = string.Empty;
    }
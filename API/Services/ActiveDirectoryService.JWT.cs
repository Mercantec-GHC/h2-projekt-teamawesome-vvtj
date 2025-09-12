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
         private readonly IConfiguration _configuration;
        private readonly string _secretKey;
        private readonly string _issuer;
        private readonly string _audience;

        public string GenerateTokenForADUser(ADUserInfo adUser)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_secretKey);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, adUser.SamAccountName),
                new Claim(ClaimTypes.Email, adUser.Email),
                new Claim("userId", adUser.SamAccountName),
                new Claim("username", adUser.SamAccountName),
                new Claim("adUser", "true"), // Marker som AD bruger
                new Claim("adGroups", string.Join(",", adUser.Groups))
            };

            // Tilføj rolle claim
            // claims.Add(new Claim(ClaimTypes.Role, role));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Issuer = _issuer,
                Audience = _audience,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}

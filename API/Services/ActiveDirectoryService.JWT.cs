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

            //Define claims for the user to include in the token
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, adUser.SamAccountName),
                new Claim(ClaimTypes.Email, adUser.Email),
                new Claim("userId", adUser.SamAccountName),
                new Claim("username", adUser.SamAccountName),
                new Claim("adUser", "true"), //Indicate that the user is AD-authenticated
                new Claim("adGroups", string.Join(",", adUser.Groups))
            };

            // claims.Add(new Claim(ClaimTypes.Role, role));

            //Create token descriptor that defines the tokens content and security
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims), //Add claims to the token
                Issuer = _issuer,                     //Who issed the token
                Audience = _audience,                 //Who the token is intended for
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),    //Use secret key
                    SecurityAlgorithms.HmacSha256Signature) 
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}

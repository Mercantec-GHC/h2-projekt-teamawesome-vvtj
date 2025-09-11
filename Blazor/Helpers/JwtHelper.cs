using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Blazor.Helpers
{
	public class JwtHelper
	{
		private readonly JwtSecurityTokenHandler _tokenHandler = new();

		//Helper method to parse claims from JWT
		public IEnumerable<Claim> GetClaimsFromJwt(string jwt)
		{
			if (string.IsNullOrWhiteSpace(jwt))
				return Enumerable.Empty<Claim>();

			var token = _tokenHandler.ReadJwtToken(jwt);
			return token.Claims;
		}
	}
}

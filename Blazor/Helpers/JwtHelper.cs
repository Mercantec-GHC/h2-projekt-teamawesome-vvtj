using System.IdentityModel.Tokens.Jwt;

namespace Blazor.Helpers;

public static class JWTHelper
{
	public static bool IsTokenExpired(string token)
	{
		if (string.IsNullOrWhiteSpace(token))
			return true;

		var jwtHandler = new JwtSecurityTokenHandler();
		if (!jwtHandler.CanReadToken(token))
			return true;

		var jwtToken = jwtHandler.ReadJwtToken(token);
		var expiration = jwtToken.ValidTo.AddHours(2);
		var dif = DateTime.UtcNow.AddHours(2) - expiration;
		var expired = expiration < DateTime.UtcNow.AddHours(2);

		Console.WriteLine($"IsTokenExpired check for token: {expired}");
		Console.WriteLine($"Token expiration: {expiration}, Current time: {DateTime.UtcNow.AddHours(2)}, Difference: {dif.TotalMinutes} minutes");
		return expired;
	}
}
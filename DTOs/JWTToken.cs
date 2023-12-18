using System.IdentityModel.Tokens.Jwt;

namespace Golbaus_BE.DTOs
{
	public class JWTToken
	{
		public static string? JwtAudienceId { get; set; }
		public static string? JwtKey { get; set; }
		public static string? JwtIssuer { get; set; }
		public static string? JwtExpireDays { get; set; }
	}

	public class TokenModel
	{
		public string access_token { get; set; }
		public string token_type { get; set; }
		public double expires_in { get; set; }
		public TokenModel() { }
		public TokenModel(string token, JwtSecurityToken properties)
		{
			access_token = token;
			token_type = "bearer";
			expires_in = Math.Floor(properties.ValidTo.Subtract(properties.ValidFrom).TotalMinutes);
		}
	}
}

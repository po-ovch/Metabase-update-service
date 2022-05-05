using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace Service_for_metabase.Model;

public static class TokenGenerator
{
	public static string GetToken(IEnumerable<Claim> claims)
	{
		var now = DateTime.UtcNow;
		
		var jwt = new JwtSecurityToken(
			JwtAuthenticationOptions.Issuer,
			JwtAuthenticationOptions.Audience,
			notBefore: now,
			claims: claims,
			expires: now.Add(TimeSpan.FromMinutes(JwtAuthenticationOptions.Lifetime)),
			signingCredentials: new SigningCredentials(JwtAuthenticationOptions.GetSymmetricSecurityKey(),
				SecurityAlgorithms.HmacSha256));
		var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

		return encodedJwt;
	}
}
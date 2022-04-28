using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Service_for_database.Model;

public static class AuthenticationOptions
{
	public const string Issuer = "MetabaseService";
	public const string Audience = "DatabaseService";
	private const string Key = "mysupersecret_secretkey!123";
	public static SymmetricSecurityKey GetSymmetricSecurityKey()
	{
		return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Key));
	}
}
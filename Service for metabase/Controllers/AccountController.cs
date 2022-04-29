using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Service_for_metabase.Model;

namespace Service_for_metabase.Controllers;

public class AccountController : Controller
{
	[HttpPost("/authenticate")]
	public async Task<IActionResult> Authenticate([FromBody] ServiceUserDto user)
	{
		var identity = GetIdentity(user.Username, user.Password);
		if (identity is null)
			return Forbid();
		
		await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, 
			new ClaimsPrincipal(identity));
		return Ok();
	}
	
	[Route("/logout")]
	public async Task Logout()
	{
		await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
	}

	private ClaimsIdentity? GetIdentity(string username, string password)
	{
		using var fs = new FileStream("Resources/usersInfo.txt", FileMode.OpenOrCreate);
		var users = JsonSerializer.Deserialize<List<ServiceUser>>(fs);
		var foundUser = users!.Find(user => user.Username == username && user.Password == password);

		if (foundUser is null)
			return null;

		var claims = new List<Claim>
		{
			new (ClaimsIdentity.DefaultNameClaimType, foundUser.Username),
			new (ClaimsIdentity.DefaultRoleClaimType, foundUser.Role)
		};

		var identity = new ClaimsIdentity(claims, "ApplicationCookie", 
			ClaimsIdentity.DefaultNameClaimType,
			ClaimsIdentity.DefaultRoleClaimType);
		return identity;
	}

	[Route("/create-users")]
	public void SaveUsers()
	{
		var first = new ServiceUser()
		{
			Username = "Admin",
			Password = "12345",
			Role = "admin"
		};
		
		var second = new ServiceUser()
		{
			Username = "SimpleUser",
			Password = "11111",
			Role = "user"
		};

		var list = new List<ServiceUser>() {first, second};
		var fs = new FileStream("Resources/usersInfo.txt", FileMode.OpenOrCreate);
		JsonSerializer.Serialize(fs, list);
	}
}
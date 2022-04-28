using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace Service_for_metabase.Controllers;

public class AccountController : Controller
{
	[Route("/authenticate")]
	public async Task Authenticate()
	{
		var claims = new List<Claim>
		{
			new (ClaimsIdentity.DefaultNameClaimType, "user.Email"),
			new (ClaimsIdentity.DefaultRoleClaimType, "role")
		};

		var identity = new ClaimsIdentity(claims, "ApplicationCookie", 
			ClaimsIdentity.DefaultNameClaimType,
			ClaimsIdentity.DefaultRoleClaimType);
		
		await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, 
			new ClaimsPrincipal(identity));
	}
	
	[Route("/logout")]
	public async Task Logout()
	{
		await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
	}
}
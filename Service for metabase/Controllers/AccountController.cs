using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Service_for_metabase.Model;

namespace Service_for_metabase.Controllers;

public class AccountController : Controller
{
	[HttpGet("/login")]
	public IActionResult Login()
	{
		if (User.Identity is {IsAuthenticated: true})
		{
			return RedirectToAction("Index", "Metabase");
		}
		return View();
	}
	
	[HttpPost("/login")]
	public async Task<IActionResult> Login(ServiceUserDto userDto)
	{
		if (User.Identity is {IsAuthenticated: true})
		{
			return RedirectToAction("Index", "Metabase");
		}
		if (ModelState.IsValid)
		{
			ServiceUser? user;
			try
			{
				user = GetUser(userDto.Username, userDto.Password);
			}
			catch
			{
				return Problem(detail: "Problem with access to users information",
					statusCode: StatusCodes.Status500InternalServerError);
			}

			if (user is not null)
			{
				await SignUserIn(user);
				return RedirectToAction("Index", "Metabase");
			}
			ModelState.AddModelError("", "Некорректные имя пользователя" +
			                             " и(или) пароль");
		}

		return View(userDto);
	}

	[Route("/logout")]
	public async Task<IActionResult> Logout()
	{
		await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
		return RedirectToAction("Login");
	}

	private async Task SignUserIn(ServiceUser user)
	{
		var claims = new List<Claim>
		{
			new (ClaimsIdentity.DefaultNameClaimType, user.Username),
			new (ClaimsIdentity.DefaultRoleClaimType, user.Role)
		};

		var identity = new ClaimsIdentity(claims, "ApplicationCookie", 
			ClaimsIdentity.DefaultNameClaimType,
			ClaimsIdentity.DefaultRoleClaimType);
		
		await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
			new ClaimsPrincipal(identity));
	}

	private static ServiceUser? GetUser(string username, string password)
	{
		using var fs = new FileStream("Resources/usersInfo.txt", FileMode.OpenOrCreate);
		var users = JsonSerializer.Deserialize<List<ServiceUser>>(fs);
		var foundUser = users!.Find(user => user.Username == username && user.Password == password);

		return foundUser;
	}
}
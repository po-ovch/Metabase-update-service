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
	public async Task<IActionResult> Login(ServiceUserModel userModel)
	{
		if (User.Identity is {IsAuthenticated: true})
		{
			return RedirectToAction("Index", "Metabase");
		}
		if (ModelState.IsValid)
		{
			var user = GetUser(userModel.Username, userModel.Password);
			if (user is not null)
			{
				await Authenticate(user);
				return RedirectToAction("Index", "Metabase");
			}
			ModelState.AddModelError("", "Некорректные имя пользователя" +
			                             " и(или) пароль");
		}

		return View(userModel);
	}

	[Route("/block")]
	public ActionResult ForbidResult()
	{
		return Problem(
			type: "/docs/errors/forbidden",
			title: "Authenticated user is not authorized.",
			detail: $"Hii",
			statusCode: StatusCodes.Status403Forbidden,
			instance: HttpContext.Request.Path
		);
	}

	[Route("/logout")]
	public async Task<ActionResult> Logout()
	{
		await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
		return RedirectToAction("Login");
	}

	private async Task Authenticate(ServiceUser user)
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
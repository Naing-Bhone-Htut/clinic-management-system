using ClinicManagementSystem.Filters;
using ClinicManagementSystem.Services;
using ClinicManagementSystem.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace ClinicManagementSystem.Controllers;

public class AccountController : Controller
{
    private readonly AuthService _auth;

    public AccountController(AuthService auth) => _auth = auth;

    [HttpGet]
    public IActionResult Login()
    {
        if (HttpContext.Session.IsLoggedIn())
            return RedirectToAction("Index", "Home");
        return View(new LoginViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);
        var user = await _auth.LoginAsync(model);
        if (user == null)
        {
            ModelState.AddModelError(string.Empty, "Invalid username or password.");
            return View(model);
        }
        HttpContext.Session.SetUserSession(user);
        return RedirectToAction("Index", "Home");
    }

    [SessionAuthorize]
    public IActionResult Logout()
    {
        HttpContext.Session.ClearUserSession();
        return RedirectToAction(nameof(Login));
    }

    public IActionResult AccessDenied() => View();
}


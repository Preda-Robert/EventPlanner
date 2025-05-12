using System.Web.Mvc;
using EventPlanner.Models;
using Microsoft.AspNetCore.Identity; // If you are using Identity
using Microsoft.AspNetCore.Mvc;

public class AccountController : Microsoft.AspNetCore.Mvc.Controller
{
  private readonly UserManager<ApplicationUser> _userManager;
  private readonly SignInManager<ApplicationUser> _signInManager;

  public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
  {
    _userManager = userManager;
    _signInManager = signInManager;
  }

  // GET: /Account/Register
  public IActionResult Register()
  {
    return View();
  }

  // POST: /Account/Register
  [Microsoft.AspNetCore.Mvc.HttpPost]
  public async Task<IActionResult> Register(RegisterViewModel model)
  {
    if (ModelState.IsValid)
    {
      var user = new ApplicationUser { UserName = model.Username, Email = model.Email };
      user.Name = model.Username;
      var result = await _userManager.CreateAsync(user, model.Password);

      if (result.Succeeded)
      {
        await _signInManager.SignInAsync(user, isPersistent: false);
        return RedirectToAction("Index", "Home");
      }
      foreach (var error in result.Errors)
      {
        ModelState.AddModelError("", error.Description);
      }
    }
    return View(model);
  }

  // GET: /Account/Login
  public IActionResult Login()
  {
    return View();
  }

  // POST: /Account/Login
  [Microsoft.AspNetCore.Mvc.HttpPost]
  public async Task<IActionResult> Login(LoginViewModel model)
  {
    if (ModelState.IsValid)
    {
      var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, model.RememberMe, lockoutOnFailure: false);

      if (result.Succeeded)
      {
        return RedirectToAction("Index", "Home");
      }

      ModelState.AddModelError("", "Invalid login attempt.");
    }
    return View(model);
  }

  [Microsoft.AspNetCore.Mvc.HttpPost]
  public async Task<IActionResult> Logout()
  {
    await _signInManager.SignOutAsync();
    return RedirectToAction("Index", "Home");
  }
}

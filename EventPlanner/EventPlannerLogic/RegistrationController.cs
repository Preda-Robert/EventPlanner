using EventPlanner.Models;
using EventPlanner.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Authorize]
public class RegistrationController : Controller
{
  private readonly IRepositoryWrapper _repo;
  private readonly UserManager<ApplicationUser> _userManager;

  public RegistrationController(IRepositoryWrapper repo, UserManager<ApplicationUser> userManager)
  {
    _repo = repo;
    _userManager = userManager;
  }

  public async Task<IActionResult> Index()
  {
    var user = await _userManager.GetUserAsync(User);
    if (user == null) return Unauthorized();

    var userRegistrations = await _repo.Registration.GetAllAsync(
        filter: r => r.UserId == user.Id,
        include: r => r.Include(r => r.Event)
    );

    return View(userRegistrations);
  }

  [HttpPost]
  [ValidateAntiForgeryToken]
  public async Task<IActionResult> Unregister(int id)
  {
    var reg = await _repo.Registration.GetByIdAsync(id);
    if (reg == null) return NotFound();

    _repo.Registration.Delete(reg);
    await _repo.SaveAsync();

    return RedirectToAction(nameof(Index));
  }
}

using EventPlanner.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class HostController : Controller
{
  private readonly IRepositoryWrapper _repo;

  public HostController(IRepositoryWrapper repo)
  {
    _repo = repo;
  }

  public async Task<IActionResult> Index()
  {
    var hosts = await _repo.Host.GetAllAsync();
    return View(hosts);
  }

  public async Task<IActionResult> Details(int id)
  {
    var host = await _repo.Host.GetByIdAsync(
    e => e.HostId == id,
    include: q => q.Include(e => e.EventsHosted)
);

    if (host == null) return NotFound();
    return View(host);
  }
  [Authorize(Roles = "Admin")]
  public IActionResult Create() => View();
  [Authorize(Roles = "Admin")]
  [HttpPost]
  [ValidateAntiForgeryToken]
  public async Task<IActionResult> Create(EventPlanner.Models.Host host)
  {
    if (ModelState.IsValid)
    {
      await _repo.Host.AddAsync(host);
      await _repo.SaveAsync();
      return RedirectToAction(nameof(Index));
    }
    return View(host);
  }
  [Authorize(Roles = "Admin")]
  public async Task<IActionResult> Edit(int id)
  {
    var host = await _repo.Host.GetByIdAsync(id);
    if (host == null) return NotFound();
    return View(host);
  }
  [Authorize(Roles = "Admin")]
  [HttpPost]
  [ValidateAntiForgeryToken]
  public async Task<IActionResult> Edit(int id, EventPlanner.Models.Host host)
  {
    if (id != host.HostId) return NotFound();

    if (ModelState.IsValid)
    {
      _repo.Host.Update(host);
      await _repo.SaveAsync();
      return RedirectToAction(nameof(Index));
    }
    return View(host);
  }
  [Authorize(Roles = "Admin")]
  public async Task<IActionResult> Delete(int id)
  {
    var host = await _repo.Host.GetByIdAsync(id);
    _repo.Host.Delete(host);
    await _repo.SaveAsync();
    return RedirectToAction(nameof(Index));
  }
  [Authorize(Roles = "Admin")]
  [HttpPost, ActionName("Delete")]
  [ValidateAntiForgeryToken]
  public async Task<IActionResult> DeleteConfirmed(int id)
  {
    var host = await _repo.Host.GetByIdAsync(id);
    _repo.Host.Delete(host);
    await _repo.SaveAsync();
    return RedirectToAction(nameof(Index));
  }
}

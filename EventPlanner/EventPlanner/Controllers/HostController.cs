using EventPlanner.Repository.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EventPlanner.Controllers
{
  public class HostController : Controller
  {
    private readonly IRepositoryWrapper _repo;

    public HostController(IRepositoryWrapper repo)
    {
      _repo = repo;
    }

    // GET: /Host
    [HttpGet]
    public async Task<IActionResult> Index()
    {
      var hosts = await _repo.Host.GetAllAsync();
      return View(hosts);
    }

    // GET: /Host/Details/5
    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
      var host = await _repo.Host.GetByIdAsync(id);
      if (host == null) return NotFound();
      return View(host);
    }

    // GET: /Host/Create
    [HttpGet]
    public IActionResult Create()
    {
      return View();
    }

    // POST: /Host/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Models.Host host)
    {
      if (ModelState.IsValid)
      {
        await _repo.Host.AddAsync(host);
        await _repo.SaveAsync();
        return RedirectToAction(nameof(Index));
      }
      return View(host);
    }

    // GET: /Host/Edit/5
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
      var host = await _repo.Host.GetByIdAsync(id);
      if (host == null) return NotFound();
      return View(host);
    }

    // POST: /Host/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Models.Host host)
    {
      if (id != host.HostId) return BadRequest();

      if (ModelState.IsValid)
      {
        _repo.Host.Update(host);
        await _repo.SaveAsync();
        return RedirectToAction(nameof(Index));
      }
      return View(host);
    }

    // GET: /Host/Delete/5
    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
      var host = await _repo.Host.GetByIdAsync(id);
      if (host == null) return NotFound();
      return View(host);
    }

    // POST: /Host/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
      var host = await _repo.Host.GetByIdAsync(id);
      if (host != null)
      {
        _repo.Host.Delete(host);
        await _repo.SaveAsync();
      }
      return RedirectToAction(nameof(Index));
    }
  }
}

namespace EventPlanner.Controllers
{
  using global::EventPlanner.Models;
  using global::EventPlanner.Repository.Interfaces;
  using Microsoft.AspNetCore.Mvc;
  using Microsoft.AspNetCore.Mvc.Rendering;
  using System.Threading.Tasks;

  namespace EventPlanner.Controllers
  {
    public class EventController : Controller
    {
      private readonly IRepositoryWrapper _repo;

      public EventController(IRepositoryWrapper repo)
      {
        _repo = repo;
      }

      // GET: /Events
      [HttpGet]
      public async Task<IActionResult> Index()
      {
        var events = await _repo.Event.GetAllAsync();
        return View(events);
      }

      // GET: /Events/Details/5
      [HttpGet]
      public async Task<IActionResult> Details(int id)
      {
        var ev = await _repo.Event.GetByIdAsync(id);
        if (ev == null) return NotFound();
        return View(ev);
      }

      // GET: /Events/Create
      [HttpGet]
      public async Task<IActionResult> Create()
      {
        // Get a list of all Hosts
        var hosts = await _repo.Host.GetAllAsync();

        // Pass the list of Hosts to the view
        ViewBag.Hosts = new SelectList(hosts, "HostId", "Name");

        return View();
      }

      // POST: /Events/Create
      [HttpPost]
      [ValidateAntiForgeryToken]
      public async Task<IActionResult> Create(Event ev)
      {
        if (ModelState.IsValid)
        {
          await _repo.Event.AddAsync(ev);
          await _repo.SaveAsync();
          return RedirectToAction(nameof(Index));
        }

        // If the model is invalid, re-populate the Host dropdown and return to the view
        var hosts = await _repo.Host.GetAllAsync();
        ViewBag.Hosts = new SelectList(hosts, "HostId", "Name");

        return View(ev);
      }
    }
  }

}

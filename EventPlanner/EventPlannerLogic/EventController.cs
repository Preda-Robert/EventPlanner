using EventPlanner.Models;
using EventPlanner.Repository.Interfaces;
using EventPlanner.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

public class EventController : Controller
{
  private readonly IRepositoryWrapper _repo;
  private readonly UserManager<ApplicationUser> _userManager;
  private readonly IEventService _eventService;

  public EventController(IRepositoryWrapper repo, UserManager<ApplicationUser> userManager, IEventService eventService)
  {
    _repo = repo;
    _userManager = userManager;
    _eventService = eventService;
  }

  [HttpPost]
  [Authorize]
  [ValidateAntiForgeryToken]
  public async Task<IActionResult> Register(int id)
  {
    var user = await _userManager.GetUserAsync(User);
    if (user == null) return Unauthorized();

    var existingReg = (await _repo.Registration.GetAllAsync())
                      .FirstOrDefault(r => r.UserId == user.Id && r.EventId == id);
    if (existingReg != null)
    {
      TempData["Message"] = "You are already registered for this event.";
      return RedirectToAction(nameof(Index));
    }

    var reg = new Registration
    {
      EventId = id,
      UserId = user.Id
    };

    await _repo.Registration.AddAsync(reg);
    await _repo.SaveAsync();

    TempData["Message"] = "Successfully registered!";
    return RedirectToAction(nameof(Index));
  }

  //public async Task<IActionResult> Index()
  //{
  //  var events = await _repo.Event.GetAllAsync(include: q => q.Include(e => e.Host));

  //  return View(events);
  //}

  public async Task<IActionResult> Index(string searchTerm, string sortOrder)
  {
    var events = await _repo.Event.GetAllAsync(include: q => q.Include(e => e.Host));
    if (!string.IsNullOrEmpty(searchTerm))
      events = _eventService.SearchEvents(events.AsQueryable(), searchTerm);
    if (!string.IsNullOrEmpty(sortOrder))
      events = _eventService.SortEvents(events.AsQueryable(), sortOrder);
    ViewBag.CurrentSearch = searchTerm;
    ViewBag.CurrentSort = sortOrder;
    return View(events);
  }


  public async Task<IActionResult> Details(int id)
  {
    var ev = await _repo.Event.GetByIdAsync(
        e => e.EventId == id,
        include: q => q.Include(e => e.Host)
                       .Include(e => e.Comments)
                           .ThenInclude(c => c.User)
    );

    if (ev == null) return NotFound();
    return View(ev);
  }

  [Authorize(Roles = "Admin")]
  public async Task<IActionResult> Create()
  {
    ViewBag.Hosts = new SelectList(await _repo.Host.GetAllAsync(), "HostId", "Name");
    ViewBag.Guests = await _repo.Guest.GetAllAsync();  // Fetch the list of guests
    return View();
  }
  //[Authorize(Roles = "Admin")]
  //[HttpPost]
  //[ValidateAntiForgeryToken]
  //public async Task<IActionResult> Create(Event ev)
  //{
  //  if (ModelState.IsValid)
  //  {
  //    await _repo.Event.AddAsync(ev);
  //    await _repo.SaveAsync();



  //    return RedirectToAction(nameof(Index));
  //  }
  //  ViewBag.Hosts = new SelectList(await _repo.Host.GetAllAsync(), "HostId", "Name", ev.HostId);
  //  return View(ev);
  //}

  [Authorize(Roles = "Admin")]
  [HttpPost]
  [ValidateAntiForgeryToken]
  public async Task<IActionResult> Create(Event ev, int[] selectedGuests)
  {
    if (ModelState.IsValid)
    {
      await _repo.Event.AddAsync(ev);
      await _repo.SaveAsync();

      if (selectedGuests != null && selectedGuests.Any())
      {
        foreach (var guestId in selectedGuests)
        {
          var eventGuest = new EventGuest
          {
            EventId = ev.EventId,
            GuestId = guestId
          };
          await _repo.EventGuest.AddAsync(eventGuest);
        }
        await _repo.SaveAsync();
      }

      return RedirectToAction(nameof(Index));
    }

    // Reload the hosts and guests if the form is invalid
    ViewBag.Hosts = new SelectList(await _repo.Host.GetAllAsync(), "HostId", "Name", ev.HostId);
    ViewBag.Guests = await _repo.Guest.GetAllAsync();
    return View(ev);
  }
  [Authorize(Roles = "Admin")]
  public async Task<IActionResult> Edit(int id)
  {
    var ev = await _repo.Event.GetByIdAsync(
        e => e.EventId == id,
        include: q => q.Include(e => e.EventGuests)
    );

    if (ev == null) return NotFound();

    // Get all guests
    var allGuests = await _repo.Guest.GetAllAsync();

    // Get the IDs of guests already assigned to this event
    var selectedGuestIds = ev.EventGuests?.Select(eg => eg.GuestId).ToList() ?? new List<int>();

    ViewBag.Hosts = new SelectList(await _repo.Host.GetAllAsync(), "HostId", "Name", ev.HostId);
    ViewBag.Guests = allGuests;
    ViewBag.SelectedGuestIds = selectedGuestIds;

    return View(ev);
  }
  [Authorize(Roles = "Admin")]
  [HttpPost]
  [ValidateAntiForgeryToken]
  public async Task<IActionResult> Edit(int id, Event ev, int[] selectedGuests)
  {
    if (id != ev.EventId) return NotFound();

    if (ModelState.IsValid)
    {
      _repo.Event.Update(ev);

      var existingEventGuests = (await _repo.EventGuest.GetAllAsync())
          .Where(eg => eg.EventId == id)
          .ToList();

      foreach (var existingGuest in existingEventGuests)
      {
        if (selectedGuests == null || !selectedGuests.Contains(existingGuest.GuestId))
        {
          _repo.EventGuest.Delete(existingGuest);
        }
      }

      if (selectedGuests != null)
      {
        foreach (var guestId in selectedGuests)
        {
          if (!existingEventGuests.Any(eg => eg.GuestId == guestId))
          {
            var eventGuest = new EventGuest
            {
              EventId = ev.EventId,
              GuestId = guestId
            };
            await _repo.EventGuest.AddAsync(eventGuest);
          }
        }
      }

      await _repo.SaveAsync();
      return RedirectToAction(nameof(Index));
    }

    ViewBag.Hosts = new SelectList(await _repo.Host.GetAllAsync(), "HostId", "Name", ev.HostId);
    ViewBag.Guests = await _repo.Guest.GetAllAsync();
    ViewBag.SelectedGuestIds = selectedGuests ?? new int[0];
    return View(ev);
  }
  [Authorize(Roles = "Admin")]
  public async Task<IActionResult> Delete(int id)
  {
    var ev = await _repo.Event.GetByIdAsync(id);
    _repo.Event.Delete(ev);
    await _repo.SaveAsync();
    return RedirectToAction(nameof(Index));
  }
  [Authorize(Roles = "Admin")]
  [HttpPost, ActionName("Delete")]
  [ValidateAntiForgeryToken]
  public async Task<IActionResult> DeleteConfirmed(int id)
  {
    var ev = await _repo.Event.GetByIdAsync(id);
    _repo.Event.Delete(ev);
    await _repo.SaveAsync();
    return RedirectToAction(nameof(Index));
  }

  [Authorize(Roles = "Admin")]
  public async Task<IActionResult> ViewGuests(int eventId)
  {
    var ev = await _repo.Event.GetByIdAsync(
        e => e.EventId == eventId,
        include: q => q.Include(e => e.EventGuests)
                      .ThenInclude(eg => eg.Guest)
                      .Include(e => e.Host)
    );

    if (ev == null)
    {
      return NotFound();
    }
    if (ev.EventGuests == null)
    {
      ev.EventGuests = new List<EventGuest>();
    }
    return View(ev);
  }


}

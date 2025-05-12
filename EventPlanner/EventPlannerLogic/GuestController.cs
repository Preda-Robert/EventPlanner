using EventPlanner.Enums;
using EventPlanner.Models;
using EventPlanner.Repository.Interfaces;
//using EventPlanner.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

public class GuestController : Controller
{
  private readonly IRepositoryWrapper _repo;

  public GuestController(IRepositoryWrapper repo)
  {
    _repo = repo;
  }

  //public async Task<IActionResult> Index()
  //{
  //  var guests = await _repo.Guest.GetAllAsync();

  //  var guestViewModels = guests.Select(g => new GuestViewModel
  //  {
  //    GuestId = g.GuestId,
  //    Name = g.Name,
  //    Role = g.Role,
  //    Type = (GuestType)g.Type
  //  }).ToList();

  //  return View(guestViewModels);
  //}


  public async Task<IActionResult> Index(int? eventId)
  {
    var guests = await _repo.Guest.GetAllAsync();

    if (eventId.HasValue)
    {
      var eventGuests = await _repo.EventGuest.GetAllAsync();
      var guestIdsForEvent = eventGuests
                              .Where(eg => eg.EventId == eventId)
                              .Select(eg => eg.GuestId)
                              .ToHashSet();

      guests = guests.Where(g => guestIdsForEvent.Contains(g.GuestId)).ToList();
    }

    var guestViewModels = guests.Select(g => new GuestViewModel
    {
      GuestId = g.GuestId,
      Name = g.Name,
      Role = g.Role,
      Type = (GuestType)g.Type
    }).ToList();

    var upcomingEvents = (await _repo.Event.GetAllAsync());

    ViewBag.Events = new SelectList(upcomingEvents, "EventId", "Title", eventId);

    return View(guestViewModels);
  }



  public async Task<IActionResult> Details(int id)
  {
    var guest = await _repo.Guest.GetByIdAsync(id);
    if (guest == null) return NotFound();
    return View(guest);
  }
  [Authorize(Roles = "Admin")]
  public IActionResult Create()
  {
    var viewModel = new GuestCreateViewModel
    {
      GuestTypes = GetGuestTypes()
    };
    return View(viewModel);
  }

  private List<SelectListItem> GetGuestTypes()
  {
    return new List<SelectListItem>
    {
        new SelectListItem { Value = GuestType.Single.ToString(), Text = "Single" },
        new SelectListItem { Value = GuestType.Multiple.ToString(), Text = "Multiple" }
    };
  }
  [Authorize(Roles = "Admin")]
  [HttpPost]
  [ValidateAntiForgeryToken]
  public async Task<IActionResult> Create(GuestCreateViewModel viewModel)
  {
    if (ModelState.IsValid)
    {
      var guest = new Guest
      {
        Name = viewModel.Name,
        Role = viewModel.Role,
        Type = (int)viewModel.Type
      };

      await _repo.Guest.AddAsync(guest);
      await _repo.SaveAsync();

      return RedirectToAction(nameof(Index));
    }

    // If model validation fails, refill the GuestTypes list
    viewModel.GuestTypes = GetGuestTypes();
    return View(viewModel);
  }
  [Authorize(Roles = "Admin")]
  public async Task<IActionResult> Edit(int id)
  {
    var guest = await _repo.Guest.GetByIdAsync(g => g.GuestId == id);
    if (guest == null)
    {
      return NotFound();
    }

    var viewModel = new GuestEditViewModel
    {
      Id = guest.GuestId,
      Name = guest.Name,
      Role = guest.Role,
      Type = (GuestType)guest.Type,
      GuestTypes = GetGuestTypes()
    };

    return View(viewModel);
  }
  [Authorize(Roles = "Admin")]
  [HttpPost]
  [ValidateAntiForgeryToken]
  public async Task<IActionResult> Edit(int id, GuestEditViewModel viewModel)
  {
    if (id != viewModel.Id)
    {
      return NotFound();
    }

    if (ModelState.IsValid)
    {
      var guest = await _repo.Guest.GetByIdAsync(g => g.GuestId == id);
      if (guest == null)
      {
        return NotFound();
      }

      guest.Name = viewModel.Name;
      guest.Role = viewModel.Role;
      guest.Type = (int)viewModel.Type;

      _repo.Guest.Update(guest);
      await _repo.SaveAsync();

      return RedirectToAction(nameof(Index));
    }

    // If validation fails, refill the dropdown
    viewModel.GuestTypes = GetGuestTypes();
    return View(viewModel);
  }
  [Authorize(Roles = "Admin")]
  public async Task<IActionResult> Delete(int id)
  {
    var guest = await _repo.Guest.GetByIdAsync(id);
    if (guest == null) return NotFound();
    return View(guest);
  }
  [Authorize(Roles = "Admin")]
  [HttpPost, ActionName("Delete")]
  [ValidateAntiForgeryToken]
  public async Task<IActionResult> DeleteConfirmed(int id)
  {
    var guest = await _repo.Guest.GetByIdAsync(id);
    if (guest == null) return NotFound();
    _repo.Guest.Delete(guest);
    await _repo.SaveAsync();
    return RedirectToAction(nameof(Index));
  }
}

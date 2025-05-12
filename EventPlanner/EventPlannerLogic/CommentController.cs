using EventPlanner.Models;
using EventPlanner.Repository.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

public class CommentController : Controller
{
  private readonly IRepositoryWrapper _repo;
  private readonly UserManager<ApplicationUser> _userManager;

  public CommentController(IRepositoryWrapper repo, UserManager<ApplicationUser> userManager)
  {
    _repo = repo;
    _userManager = userManager;
  }

  public async Task<IActionResult> Index()
  {
    var comments = await _repo.Comment.GetAllAsync();
    return View(comments);
  }

  public async Task<IActionResult> Details(int id)
  {
    var comment = await _repo.Comment.GetByIdAsync(id);
    if (comment == null) return NotFound();
    return View(comment);
  }

  public IActionResult Create()
  {
    return View();
  }

  [HttpPost]
  [ValidateAntiForgeryToken]
  public async Task<IActionResult> Create(Comment comment)
  {
    var user = await _userManager.GetUserAsync(User);
    comment.UserId = user.Id;
    comment.User = user;
    comment.Event = await _repo.Event.GetByIdAsync(comment.EventId);
    ModelState.Remove("User");
    ModelState.Remove("Event");
    if (ModelState.IsValid)
    {
      comment.CreatedAt = DateTime.Now;
      await _repo.Comment.AddAsync(comment);
      await _repo.SaveAsync();
      return RedirectToAction("Details", "Event", new { id = comment.EventId });
    }
    //return View(comment);
    return RedirectToAction("Details", "Event", new { id = comment.EventId });
  }


  public async Task<IActionResult> Edit(int id)
  {
    var comment = await _repo.Comment.GetByIdAsync(id);
    if (comment == null) return NotFound();
    return View(comment);
  }

  [HttpPost]
  [ValidateAntiForgeryToken]
  public async Task<IActionResult> Edit(int id, Comment comment)
  {
    if (id != comment.CommentId) return NotFound();

    if (ModelState.IsValid)
    {
      _repo.Comment.Update(comment);
      await _repo.SaveAsync();
      return RedirectToAction(nameof(Index));
    }
    return View(comment);
  }

  public async Task<IActionResult> Delete(int id)
  {
    var comment = await _repo.Comment.GetByIdAsync(id);
    if (comment == null) return NotFound();
    return View(comment);
  }

  [HttpPost, ActionName("Delete")]
  [ValidateAntiForgeryToken]
  public async Task<IActionResult> DeleteConfirmed(int id)
  {
    var comment = await _repo.Comment.GetByIdAsync(id);
    if (comment == null) return NotFound();
    _repo.Comment.Delete(comment);
    await _repo.SaveAsync();
    return RedirectToAction(nameof(Index));
  }
}

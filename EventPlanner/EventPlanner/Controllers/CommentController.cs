using Microsoft.AspNetCore.Mvc;

namespace EventPlanner.Controllers
{
    public class CommentController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

using Microsoft.AspNetCore.Mvc;

namespace EventPlanner.Controllers
{
    public class RegistrationController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

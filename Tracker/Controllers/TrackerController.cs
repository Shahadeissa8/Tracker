using Microsoft.AspNetCore.Mvc;

namespace Tracker.Controllers
{
    public class TrackerController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

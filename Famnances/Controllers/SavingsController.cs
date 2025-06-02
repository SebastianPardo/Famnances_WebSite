using Microsoft.AspNetCore.Mvc;

namespace Famnances.Controllers
{
    public class SavingsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

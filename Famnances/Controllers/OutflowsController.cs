using Microsoft.AspNetCore.Mvc;

namespace Famnances.Controllers
{
    public class OutflowsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

using Microsoft.AspNetCore.Mvc;

namespace Famnances.Controllers
{
    public class BudgetsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

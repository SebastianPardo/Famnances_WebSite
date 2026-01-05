using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Famnances.Controllers
{
    public class IntroductionController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        // GET: IntroductionController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: IntroductionController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}

using Famnances.DataCore.Entities;
using Famnances.Helpers;
using Famnances.Helpers.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Famnances.Controllers
{
    public class UsersController : Controller
    {
        IHttpHelper _httpHelper;
        public UsersController(IHttpHelper httpHelper)
        {
            _httpHelper = httpHelper;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Details()
        {
            var accountId = HttpContext.Session.GetString(Constants.ACCOUNT_ID);
            var user = await _httpHelper.Get<User>($"{Constants.USER_URI}/{accountId}");
            ViewBag.Countries = new SelectList(await _httpHelper.Get<List<Country>>($"{Constants.COUNTRIES_URI}"), "Id", "Name");
            ViewBag.Periods = new SelectList(await _httpHelper.Get<List<Period>>($"{Constants.PERIODS_URI}"), "Id", "Name");
            return View(user ?? new User { Id = Guid.Parse(accountId) });
        }

        [HttpPost]
        public async Task<IActionResult> Details(User entity)
        {
            entity.Id = Guid.Parse(HttpContext.Session.GetString(Constants.ACCOUNT_ID));
            User user = await _httpHelper.Post<User>($"{Constants.USER_URI}", entity);
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> GetProvincesByCountry(Guid countryId)
        {
            return Ok(await _httpHelper.Get<List<Province>>($"{Constants.PROVINCES_URI}/GetProvincesByCountry/{countryId}"));
        }

        [HttpGet]
        public async Task<IActionResult> GetCitiesByProvince(Guid provinceId)
        {
            return Ok(await _httpHelper.Get<List<Province>>($"{Constants.CITIES_URI}/GetCitiesByProvince/{provinceId}"));
        }
    }
}

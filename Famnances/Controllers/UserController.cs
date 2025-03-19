using Famnances.AuthMiddleware;
using Famnances.DataCore.Entities;
using Famnances.Helpers.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;
using Constants = Famnances.Helpers.Constants;

namespace Famnances.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        IHttpHelper _httpHelper;

        public UserController(IHttpHelper httpHelper)
        {
            _httpHelper = httpHelper;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Countries = new SelectList(await _httpHelper.Get<List<Country>>($"{Constants.MANAGEMENT_URI}/GetCountries"), "Id", "Name");
            ViewBag.Periods = new SelectList(await _httpHelper.Get<List<Country>>($"{Constants.MANAGEMENT_URI}/GetPeriods"), "Id", "Name");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(User entity)
        {
            entity.Id = Guid.Parse(HttpContext.Session.GetString(Constants.ACCOUNT_ID));
            User user = await _httpHelper.Post<User>($"{Constants.USER_URI}",entity);
            return RedirectToAction("Index","Home");
        }

        [HttpGet]
        public async Task<IActionResult> GetProvincesByCountry(Guid countryId)
        {
            return Ok(await _httpHelper.Get<List<Province>>($"{Constants.MANAGEMENT_URI}/GetProvincesByCountry/{countryId}"));
        }

        [HttpGet]
        public async Task<IActionResult> GetCitiesByProvince(Guid provinceId)
        {
            return Ok(await _httpHelper.Get<List<Province>>($"{Constants.MANAGEMENT_URI}/GetCitiesByProvince/{provinceId}"));
        }
    }
}

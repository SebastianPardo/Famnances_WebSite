using Famnances.Core.Security.Authorization;
using Famnances.DataCore.Entities;
using Famnances.Helpers;
using Famnances.Helpers.Interfaces;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace Famnances.Controllers
{
    //[ServiceFilter(typeof(AuthorizeAttribute))]
    public class LanguagesController : Controller
    {
        IHttpHelper _httpHelper;
        public LanguagesController(IHttpHelper httpHelper)
        {
            _httpHelper = httpHelper;
        }

        [HttpGet]
        public async Task<IActionResult> ChangeLanguage(string culture)
        {
            string languageCulture = "es-CO";
            switch (culture)
            {
                case "ES":
                case "es-CO":
                    languageCulture = "es-CO";
                    break;
                case "EN":
                case "en-CA":
                    languageCulture = "en-CA";
                    break;
                case "FR":
                case "fr-CA":
                    languageCulture = "fr-CA";
                    break;
                default:
                    languageCulture = "en-CA";
                    break;
            }

            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(languageCulture)),
                    new CookieOptions
                    {
                        Expires = DateTimeOffset.UtcNow.AddDays(7)
                    }
            );

            var token = HttpContext.Session.GetString(Constants.TOKEN);

            //if (token == null)
            //{
            //    return RedirectToAction(nameof(UsersController.Create), "Users");
            //}

            var currentUrl = Request.Headers["Referer"].ToString();
            var urlSplit = currentUrl.Split('/');

            if ((urlSplit.Length <= 4 || currentUrl.Contains("Landing")) && token == null)
            {
                return RedirectToAction(nameof(HomeController.Landing), "Home");
            }
            else if (string.IsNullOrWhiteSpace(currentUrl) || currentUrl.Contains("Introduction"))
            {
                return RedirectToAction(nameof(IntroductionController.Index), "Introduction");
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }
    }
}

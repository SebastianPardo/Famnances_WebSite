using Famnances.Core.Security.Authorization;
using Famnances.Helpers;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace Famnances.Controllers
{
    //[ServiceFilter(typeof(AuthorizeAttribute))]
    public class LanguagesController : Controller
    {
        [HttpGet]
        public IActionResult ChangeLanguage(string culture)
        {
            string languageCulture = "es-CO";
            switch (culture)
            {
                case "ES":
                    languageCulture = "es-CO";
                    break;
                case "EN":
                    languageCulture = "en-CA";
                    break;
                case "FR":
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

            if (token == null)
            {
                return RedirectToAction(nameof(UsersController.Create), "Users");
            }

            var currentUrl = Request.Headers["Referer"].ToString();

            switch (currentUrl)
            {
                case string url when string.IsNullOrWhiteSpace(url):
                    return RedirectToAction(nameof(IntroductionController.Language), "Introduction");
                case string url when url.Contains("Introduction"):
                    return RedirectToAction(nameof(IntroductionController.Index), "Introduction");
                case string url when url.Contains("Home"):
                    return RedirectToAction(nameof(HomeController.Index), "Home");
                default:
                    return RedirectToAction(nameof(UsersController.Create), "Users");
            }
        }
    }
}

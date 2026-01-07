using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace Famnances.Controllers
{
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

            var currentUrl = Request.Headers["Referer"].ToString();
            if (string.IsNullOrWhiteSpace(currentUrl))
                return RedirectToAction("Languages", "Introduction");

            return currentUrl.Contains("Introduction") ?
                RedirectToAction("Index", "Introduction") : RedirectToAction("Index", "Home");

        }
    }
}

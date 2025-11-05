using Azure;
using Famnances.Core.Utils.Helpers;
using Famnances.Helpers;
using Famnances.Helpers.Interfaces;
using Famnances.Models.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace Famnances.Controllers
{
    public class LoginController : Controller
    {
        IHttpHelper HttpHelper;

        public LoginController(IHttpHelper httpHelper)
        {
            HttpHelper = httpHelper;
        }

        // GET: LoginViewModel
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> Logout()
        {
            if (User.Identity.IsAuthenticated)
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            }
            HttpContext.Session.Remove(Constants.TOKEN);
            HttpContext.Session.Remove(Constants.ACCOUNT_ID);
            return Redirect("../");
        }

        [HttpPost]
        public async Task<ActionResult> Login(LoginViewModel login)
        {
            LoginResponseViewModel user = await HttpHelper.Post<LoginResponseViewModel>($"{Constants.AUTH_URI}/Authenticate", login);
            HttpContext.Session.SetString(Constants.TOKEN, user.Token);
            HttpContext.Session.SetString(Constants.ACCOUNT_ID, user.AccountId.ToString());
            return RedirectToAction("Index", "Home", new { date = DateTimeEast.Now });
        }

        public IActionResult ExternalLogin(string provider)
        {
            var redirectUrl = Url.Action("ExternalLoginCallback", new { provider });
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            return Challenge(properties, provider);
        }

        public async Task<IActionResult> ExternalLoginCallback(string provider)
        {
            var auth = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (!auth.Succeeded)
                return RedirectToAction("Logout");

            string accessToken = auth.Properties.GetTokenValue("access_token");
            string idToken = auth.Properties.GetTokenValue("id_token");

            ExternalAuthenticateViewModel request = new ExternalAuthenticateViewModel { Param_1 = provider, Param_2 = accessToken, Param_3 = idToken };


            var response = await HttpHelper.Post<LoginResponseViewModel>($"{Constants.AUTH_URI}/ExternalAuthenticate", request);
            if (response == null)
                return RedirectToAction("Logout");

            HttpContext.Session.SetString(Constants.TOKEN, response.Token);
            HttpContext.Session.SetString(Constants.ACCOUNT_ID, response.AccountId.ToString());
            if (response.IsFirstLogin)
                return RedirectToAction("Details", "Users");
            return RedirectToAction("Index", "Home", new { date = DateTimeEast.Now });
        }
    }
}

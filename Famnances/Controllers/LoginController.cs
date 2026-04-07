using Athentication.DataCore.ApiModels;
using Famnances.DataCore.Entities;
using Famnances.Helpers;
using Famnances.Helpers.Interfaces;
using Famnances.Models.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

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
            AuthResponse user = await HttpHelper.Post<AuthResponse>($"{Constants.AUTH_URI}/Authenticate", login);
            HttpContext.Session.SetString(Constants.TOKEN, user.Token);
            HttpContext.Session.SetString(Constants.ACCOUNT_ID, user.AccountId.ToString());
            return RedirectToAction("Index", "Home");
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

            AuthRequest request = new AuthRequest { Param_1 = provider, Param_2 = accessToken, Param_3 = idToken };


            var response = await HttpHelper.Post<AuthResponse>($"{Constants.AUTH_URI}/ExternalAuthenticate", request);

            if (response == null)
                return RedirectToAction("Logout");

            if (response.Token == "NO_DATABASE")
                return RedirectToAction("Offline", "Home");

            HttpContext.Session.SetString(Constants.TOKEN, response.Token);
            HttpContext.Session.SetString(Constants.ACCOUNT_ID, response.AccountId.ToString());

            if (response.IsFirstLogin)
            {
                TempData["UserInfo"] = JsonSerializer.Serialize(response.UserInfo);
                return RedirectToAction("NewUser", "Users");
            }

            return RedirectToAction("ChangeLanguage", "Languages", new { culture = response.Language });
        }

        public async Task<IActionResult> Guest()
        {
            LoginViewModel login = new LoginViewModel
            {
                Param_1 = "guest@gmail.com",
                Param_2 = "GuestUserPassword"
            };
            AuthResponse user = await HttpHelper.Post<AuthResponse>($"{Constants.AUTH_URI}/Authenticate", login);
            HttpContext.Session.SetString(Constants.TOKEN, user.Token);
            HttpContext.Session.SetString(Constants.ACCOUNT_ID, user.AccountId.ToString());
            return RedirectToAction("Index", "Home");
        }
    }
}

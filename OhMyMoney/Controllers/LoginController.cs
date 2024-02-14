using Google.Apis.Auth.OAuth2;
using Google.Apis.Oauth2.v2;
using Google.Apis.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using OhMyMoney.Business.Interfaces;
using OhMyMoney.Models;

namespace OhMyMoney.Controllers
{
    public class LoginController : Controller
    {
        IHttpHelper HttpHelper;

#if DEBUG
        public const string BASE_ROUTE_SERVICE = "https://localhost:7238/Api/";
#else        
        public const string BASE_ROUTE_SERVICE = "";
#endif

        public LoginController(IHttpHelper httpHelper)
        {
            HttpHelper = httpHelper;
        }

        // GET: Login
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> LogoutReport()
        {
            if (User.Identity.IsAuthenticated)
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            }
            HttpContext.Session.Remove("token");
            HttpContext.Session.Remove("email");
            return Redirect("../Gmail/Report");
        }

        public async Task<IActionResult> Logout()
        {
            if (User.Identity.IsAuthenticated)
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            }
            HttpContext.Session.Remove("token");
            HttpContext.Session.Remove("email");
            return Redirect("../");
        }

        [HttpPost]
        public async Task<ActionResult> Login(Login login)
        {
            var user = await HttpHelper.Post<LoginResponse>(BASE_ROUTE_SERVICE + "Account/Authenticate", login);
            HttpContext.Session.SetString("token", user.Token);
            HttpContext.Session.SetString("email", user.Email);
            return Redirect("../Home/Index");
        }

        public IActionResult GoogleLogin()
        {
            var properties = new AuthenticationProperties { RedirectUri = Url.Action("GoogleResponse") };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        public async Task<IActionResult> GoogleResponse()
        {
            var auth = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            string accessToken = auth.Properties.GetTokenValue(OpenIdConnectParameterNames.AccessToken);
            GoogleCredential cred2 = GoogleCredential.FromAccessToken(accessToken);
            var oauthSerivce = new Oauth2Service(new BaseClientService.Initializer { HttpClientInitializer = cred2 });
            var userinfo = await oauthSerivce.Userinfo.Get().ExecuteAsync();
            GoogleAuthenticateRequest googleAuthenticateRequest = new GoogleAuthenticateRequest { Param_1 = userinfo.Email, Param_2 = accessToken };
            var user = await HttpHelper.Post<LoginResponse>(BASE_ROUTE_SERVICE + "Account/GoogleAuthenticate", googleAuthenticateRequest);
            HttpContext.Session.SetString("token", user.Token);
            return Redirect("../Home/Index");
        }
    }
}

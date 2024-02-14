using Google.Apis.Auth.OAuth2;
using Google.Apis.Oauth2.v2;
using Google.Apis.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.Text.Json;
using System.Net.Http.Headers;
using OhMyMoney.DataCore.Entities;

namespace OhMyMoney.Authorization
{
    public class Authorize : Attribute, IAuthorizationFilter
    {
#if DEBUG
        public const string BASE_ROUTE_SERVICE = "";
#else
        public const string BASE_ROUTE_SERVICE = "";
#endif
        public void OnAuthorization(AuthorizationFilterContext filterContext)
        {
            Task<AuthenticateResult> authResult0 = filterContext.HttpContext.AuthenticateAsync();
            authResult0?.Wait();

            var email = filterContext.HttpContext.Session.GetString("email");
            var token = filterContext.HttpContext.Session.GetString("token");

            if (email == null)
            {
                string accessToken0 = authResult0.Result.Properties.GetTokenValue(OpenIdConnectParameterNames.AccessToken);
                GoogleCredential cred2 = GoogleCredential.FromAccessToken(accessToken0);
                var oauthSerivce = new Oauth2Service(new BaseClientService.Initializer { HttpClientInitializer = cred2 });
                var userGet = oauthSerivce.Userinfo.V2.Me.Get();
                var userinfo = userGet.Execute();
                email = userinfo.Email;
            }
;
            var request = new HttpRequestMessage(HttpMethod.Get, new Uri(BASE_ROUTE_SERVICE + "User/GetByEmail/" + email));
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            HttpClient httpClient = new HttpClient();
            var response = httpClient.Send(request);
            var options = new JsonSerializerOptions();
            options.PropertyNameCaseInsensitive = true;
            var user = response.Content.ReadFromJsonAsync<User>(options);
            user?.Wait();

            if (user == null)
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Login", action = "Index" }));
            }
        }
    }
}

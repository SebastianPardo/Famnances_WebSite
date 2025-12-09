using Famnances.Core;
using Famnances.Core.Entities;
using Famnances.Core.Errors;
using Famnances.Core.Security.Authorization;
using Famnances.Core.Security.Services;
using Famnances.Core.Security.Services.Interfaces;
using Famnances.Helpers;
using Famnances.Helpers.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Authentication.Google;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));

// Add services to the container.
builder.Services.AddControllersWithViews().AddNewtonsoftJson(x => x.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
builder.Services.AddSession();

builder.Services.AddHttpContextAccessor();

builder.Services.AddHttpClient("AuthService", client =>
{
    client.BaseAddress = new Uri(Constants.AUTH_SERVICES_URI);
});

builder.Services.AddHttpClient("FamnancesService", client =>
{
    client.BaseAddress = new Uri(Constants.FAMNACES_SERVICES_URI);
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie()
    .AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
    {
        options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
        options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
        options.SaveTokens = true;
        options.Scope.Add("openid");
        options.Scope.Add("profile");
        options.Scope.Add("email");
        //options.ResponseType = "code";
        //})
        //.AddFacebook(FacebookDefaults.AuthenticationScheme, options =>
        //{
        //    options.AppId = builder.Configuration["Authentication:Facebook:AppId"];
        //    options.AppSecret = builder.Configuration["Authentication:Facebook:AppSecret"];
        //    options.SaveTokens = true;
    });
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<ITokenHandler, TokenHandler>();
builder.Services.AddScoped<IHttpHelper, HttpHelper>();
builder.Services.AddScoped<HeaderSummaryFilter>();
builder.Services.AddScoped<AuthorizeAttribute>();

var app = builder.Build();
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseSession();

app.UseExceptionHandler("/Home/Error");

app.UseHttpsRedirection();
app.UseStaticFiles();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Index}/{id?}");

app.Run();

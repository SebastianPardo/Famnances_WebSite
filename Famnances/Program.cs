using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Famnances.Business.Interfaces;
using Famnances.DataCore.Data;
using Famnances.Helpers;
using Google.Apis.Auth.AspNetCore3;

var builder = WebApplication.CreateBuilder(args);

#if DEBUG
builder.Services.AddDbContext<DatabaseContext>(options => options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
.UseSqlServer(builder.Configuration.GetConnectionString("Database"), x => x.MigrationsAssembly("Famnances.DataCore")));
#else
builder.Services.AddDbContext<DatabaseContext>(options =>options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
.UseSqlServer(builder.Configuration.GetConnectionString("Database"), x => x.MigrationsAssembly("Famnances.DataCore")));
#endif

// Add services to the container.
builder.Services.AddControllersWithViews().AddNewtonsoftJson(x => x.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
builder.Services.AddSession();

builder.Services.AddAuthentication(o =>
{
    o.DefaultChallengeScheme = GoogleOpenIdConnectDefaults.AuthenticationScheme;
    o.DefaultForbidScheme = GoogleOpenIdConnectDefaults.AuthenticationScheme;
    o.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
      .AddCookie()
      .AddGoogleOpenIdConnect(options =>
      {
          options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
          options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
      })
      .AddGoogle(googleOptions =>
      {
          googleOptions.SaveTokens = true;
          googleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"];
          googleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
      });

builder.Services.AddScoped<IHttpHelper, HttpHelper>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Index}/{id?}");

app.Run();

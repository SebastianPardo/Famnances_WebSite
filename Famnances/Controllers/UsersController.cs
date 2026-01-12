using AspNetCoreGeneratedDocument;
using Athentication.DataCore.Models;
using Famnances.Core.Utils.Helpers;
using Famnances.DataCore.Entities;
using Famnances.Helpers;
using Famnances.Helpers.Interfaces;
using Famnances.Models.ViewModels;
using Google.Apis.Util;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Famnances.Controllers
{
    public class UsersController : Controller
    {
        IHttpHelper _httpHelper;
        IUtilities _utilities;
        public UsersController(IHttpHelper httpHelper, IUtilities utilities)
        {
            _httpHelper = httpHelper;
            _utilities = utilities;
        }

        public async Task<IActionResult> NewUser()
        {
            var json = TempData["UserInfo"] as string;
            var userInfo = JsonSerializer.Deserialize<UserInfo>(json);

            var accountId = HttpContext.Session.GetString(Constants.ACCOUNT_ID);
            var user = await _httpHelper.Get<User>($"{Constants.USER_URI}/{accountId}");

            if (user == null)
            {
                user = new User
                {
                    Id = Guid.Parse(accountId),
                    FirstName = userInfo.GivenName,
                    LastName = userInfo.FamilyName,
                    LegalName = $"{userInfo.GivenName} {userInfo.FamilyName}",
                    Address = "NO ADDRESS",
                    PostalCode = "A0B1C2",
                    PhoneNumber = "0000000000",
                    TotalSavings = 0,
                    TotalBudget = 0,
                    BudgetByPeriod = 0,
                    PeriodStartsMonthsDay = 0,
                    HomeAdministrator = false,
                    Language = "EN",
                    PeriodId = (await _httpHelper.Get<Period>($"{Constants.PERIODS_URI}/MON")).Id,
                    CityId = (await _httpHelper.Get<Period>($"{Constants.CITIES_URI}/GetByCode/NONE")).Id
                };
                user = await _httpHelper.Post<User>($"{Constants.USER_URI}", user);
            }

            return RedirectToAction("Language", "Introduction");
        }

        [HttpGet]
        public async Task<IActionResult> Details()
        {
            var accountId = HttpContext.Session.GetString(Constants.ACCOUNT_ID);
            var user = await _httpHelper.Get<User>($"{Constants.USER_URI}/{accountId}");
            ViewBag.Countries = new SelectList(await _httpHelper.Get<List<Country>>($"{Constants.COUNTRIES_URI}"), "Id", "Name");
            ViewBag.Periods = await _utilities.GetPeriodDropdown(Thread.CurrentThread.CurrentUICulture.ToString());
            return View(user ?? new User { Id = Guid.Parse(accountId) });
        }

        [HttpPost]
        public async Task<IActionResult> Details(User entity)
        {
            entity.Id = Guid.Parse(HttpContext.Session.GetString(Constants.ACCOUNT_ID));
            User user = await _httpHelper.Post<User>($"{Constants.USER_URI}", entity);
            return RedirectToAction("Index", "Home", new { date = DateTimeEast.Now.ToString("yyyy-MM-dd") });
        }

        [HttpGet]
        public async Task<IActionResult> GetProvincesByCountry(Guid countryId)
        {
            return Ok(await _httpHelper.Get<List<Province>>($"{Constants.PROVINCES_URI}/GetProvincesByCountry/{countryId}"));
        }

        [HttpGet]
        public async Task<IActionResult> GetCitiesByProvince(Guid provinceId)
        {
            return Ok(await _httpHelper.Get<List<Province>>($"{Constants.CITIES_URI}/GetCitiesByProvince/{provinceId}"));
        }



        #region Home
        public async Task<IActionResult> Invitations()
        {
            var accountId = HttpContext.Session.GetString(Constants.ACCOUNT_ID);
            var user = await _httpHelper.Get<User>($"{Constants.USER_URI}/{accountId}");
            user.Account = await _httpHelper.Get<Account>($"{Constants.ACCOUNT_URI}/{accountId}");
            if (user.HomeAdministrator)
            {
                var invitations = await _httpHelper.Get<List<HomeInvitation>>($"{Constants.HOME_URI}/GetGuestRequests/{user.HomeId}");
                return View(new SearchUserViewModel(user.HomeId, invitations));
            }
            else
            {
                var invitation = await _httpHelper.Get<List<HomeInvitation>>($"{Constants.HOME_URI}/GetInvitations");
                return View(new SearchUserViewModel(user.HomeId, invitation));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Invitations(Guest guest)
        {
            var accountId = HttpContext.Session.GetString(Constants.ACCOUNT_ID);
            var user = await _httpHelper.Get<User>($"{Constants.USER_URI}/{accountId}");
            HomeInvitation homeInvitation = new HomeInvitation
            {
                Id = Guid.NewGuid(),
                IsInvitation = user.HomeAdministrator,
                HostId = user.HomeAdministrator ? user.Id : guest.UserId,
                GuestId = user.HomeAdministrator ? guest.UserId : user.Id,
                HomeId = user.HomeId,
                InvitationDate = DateTime.Now
            };

            if (user.HomeId == null)
            {
                var administrator = await _httpHelper.Get<User>($"{Constants.USER_URI}/{guest.UserId}");
                homeInvitation.HomeId = administrator.HomeId;
            }

            var invitations = await _httpHelper.Post<List<HomeInvitation>>($"{Constants.HOME_URI}/Invite", homeInvitation);
            return View(new SearchUserViewModel(user.HomeId, invitations ?? new List<HomeInvitation>()));
        }

        [HttpGet]
        public async Task<JsonResult> GetGuests(string prefix)
        {
            List<User> guests = await _httpHelper.Get<List<User>>($"{Constants.USER_URI}/Search/{prefix}");
            return Json(guests);
        }

        [HttpGet]
        public async Task<IActionResult> AceptInvitation(Guid invitationId)
        {
            var invitations = await _httpHelper.Get<List<HomeInvitation>>($"{Constants.HOME_URI}/AcceptInvitation/{invitationId}");

            return RedirectToAction("Index", "Home", new { date = DateTimeEast.Now.ToString("yyyy-MM-dd") });
        }


        public async Task<IActionResult> CreateHome()
        {
            var accountId = HttpContext.Session.GetString(Constants.ACCOUNT_ID);
            var user = await _httpHelper.Get<User>($"{Constants.USER_URI}/{accountId}");
            var invitation = await _httpHelper.Get<List<HomeInvitation>>($"{Constants.HOME_URI}/GetInvitations");
            if (user.HomeId != null)
            {
                return RedirectToAction(nameof(EditHome), new { id = user.HomeId });
            }
            if (invitation != null && invitation.Count > 0)
            {
                return RedirectToAction(nameof(Invitations));
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateHome([Bind("Id,Name,ShareSavings,ShareExpenses,ShareIncomes")] Home home)
        {
            if (ModelState.IsValid)
            {
                home.Id = Guid.NewGuid();
                home = await _httpHelper.Post<Home>($"{Constants.HOME_URI}", home);

                var accountId = HttpContext.Session.GetString(Constants.ACCOUNT_ID);
                var user = await _httpHelper.Get<User>($"{Constants.USER_URI}/{accountId}");
                user.HomeId = home.Id;
                user.HomeAdministrator = true;
                await _httpHelper.Put($"{Constants.USER_URI}/{accountId}", user);

                return RedirectToAction("Index", "Home", new { date = DateTimeEast.Now.ToString("yyyy-MM-dd") });
            }
            return View(home);
        }

        public async Task<IActionResult> EditHome(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var home = await _httpHelper.Get<Home>($"{Constants.HOME_URI}/{id}");
            if (home == null)
            {
                return NotFound();
            }
            return View(home);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditHome(Guid id, [Bind("Id,Name,ShareSavings,ShareExpenses,ShareIncomes")] Home home)
        {
            if (id != home.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _httpHelper.Put($"{Constants.HOME_URI}/{id}", home);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (await _httpHelper.Get<Home>($"{Constants.HOME_URI}/{id}") == null)
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index", "Home", new { date = DateTimeEast.Now.ToString("yyyy-MM-dd") });
            }
            return View(home);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteHome(Guid id)
        {
            var home = await _httpHelper.Get<Home>($"{Constants.HOME_URI}/{id}");
            if (home != null)
            {
                await _httpHelper.Delete<Home>($"{Constants.HOME_URI}/{id}");
            }

            return RedirectToAction("Index", "Home", new { date = DateTimeEast.Now.ToString("yyyy-MM-dd") });
        }

        #endregion
    }
}

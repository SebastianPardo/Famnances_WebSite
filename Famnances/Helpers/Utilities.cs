using Famnances.DataCore.Entities;
using Famnances.Helpers.Interfaces;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Famnances.Helpers
{
    public class Utilities : IUtilities
    {
        IHttpHelper _httpHelper;
        public Utilities(IHttpHelper httpHelper)
        {
            _httpHelper = httpHelper;
        }
        public async Task<SelectList> GetPeriodDropdown(string language)
        {
            switch (language)
            {
                case "es-CO":
                case "ES":
                    return new SelectList(await _httpHelper.Get<List<Period>>($"{Constants.PERIODS_URI}"), "Id", "NameES");
                case "en-CA":
                case "EN":
                    return new SelectList(await _httpHelper.Get<List<Period>>($"{Constants.PERIODS_URI}"), "Id", "NameEN");
                case "FR":
                case "fr-CA":
                    return new SelectList(await _httpHelper.Get<List<Period>>($"{Constants.PERIODS_URI}"), "Id", "NameFR");
                default:
                    return new SelectList(await _httpHelper.Get<List<Period>>($"{Constants.PERIODS_URI}"), "Id", "NameEN");
            }
        }
    }
}

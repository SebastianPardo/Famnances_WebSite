using Famnances.Core.Utils.Helpers;
using Famnances.DataCore.Entities;
using Famnances.Helpers.Interfaces;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Famnances.Helpers
{
    public class LanguageHelper : ILanguageHelper
    {
        IHttpHelper _httpHelper;
        public LanguageHelper(IHttpHelper httpHelper)
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
                case "fr-CA":
                case "FR":
                    return new SelectList(await _httpHelper.Get<List<Period>>($"{Constants.PERIODS_URI}"), "Id", "NameFR");
                default:
                    return new SelectList(await _httpHelper.Get<List<Period>>($"{Constants.PERIODS_URI}"), "Id", "NameEN");
            }
        }

        public async Task<string> GetPeriodName(string language, Period period)
        {
            switch (language)
            {
                case "es-CO":
                case "ES":
                    return period.NameES;
                case "en-CA":
                case "EN":
                    return period.NameEN;
                case "fr-CA":
                case "FR":
                    return period.NameFR;
                default:
                    return period.NameEN;
            }
        }

        public decimal GetValueByPeriod(decimal value, string fromPeriod, string toPeriod)
        {
            switch (fromPeriod)
            {
                case "MON":
                    value = value * 12;
                    break;
                case "SMON":
                    value = value * 24;
                    break;
                case "BWEEK":
                    value = value * 26;
                    break;
                case "WEEK":
                    value = value * 52;
                    break;
                case "DAY":
                    value = value * 52 * 5;
                    break;
            }

            switch (toPeriod)
            {
                case "MON":
                    value = value / 12;
                    break;
                case "SMON":
                    value = value / 24;
                    break;
                case "BWEEK":
                    value = value / 26;
                    break;
                case "WEEK":
                    value = value / 52;
                    break;
                case "DAY":
                    value = value / (52 * 5);
                    break;
            }

            return value;
        }
    }
}

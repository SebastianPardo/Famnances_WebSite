using Famnances.DataCore.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Famnances.Helpers.Interfaces
{
    public interface ILanguageHelper
    {
        Task<SelectList> GetPeriodDropdown(string language);
        Task<string> GetPeriodName(string language, Period period);
        decimal GetValueByPeriod(decimal value, string fromPeriod, string toPeriod);
    }
}

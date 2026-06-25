using Famnances.DataCore.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Famnances.Helpers.Interfaces
{
    public interface ILanguageHelper
    {
        Task<SelectList> GetPeriodDropdown(string language, Guid? id = null);
        Task<SelectList> GetSourceDropdown(string language, Guid? id = null);
        Task<string> GetPeriodName(string language, Period period);
        decimal GetValueByPeriod(decimal value, string fromPeriod, string toPeriod);
    }
}

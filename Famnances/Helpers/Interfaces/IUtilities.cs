using Microsoft.AspNetCore.Mvc.Rendering;

namespace Famnances.Helpers.Interfaces
{
    public interface IUtilities
    {
        Task<SelectList> GetPeriodDropdown(string language);
    }
}

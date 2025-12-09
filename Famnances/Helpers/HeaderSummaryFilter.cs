using Famnances.DataCore.ServicesModels;
using Famnances.Helpers.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Famnances.Helpers
{
    public class HeaderSummaryFilter :  IAsyncActionFilter
    {
        IHttpHelper _httpHelper;
        public HeaderSummaryFilter(IHttpHelper httpHelper)
        {
            _httpHelper = httpHelper;
        }

        public async Task OnActionExecutionAsync (ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var date = context.HttpContext.Session.GetString(Constants.DATE_FROM);

            if (!string.IsNullOrEmpty(date))
            {
                var summary = await _httpHelper.Get<MiniSummaryModel?>($"{Constants.ACCOUNTING_URI}/GetHeaderSummary/{date}");
                if(summary != null)
                {
                    context.HttpContext.Session.SetString(Constants.CHEQUING, summary.Chequing.ToString());
                    context.HttpContext.Session.SetString(Constants.SAVINGS, summary.Savings.ToString());
                }
            }
            await next();
        }
    }
}

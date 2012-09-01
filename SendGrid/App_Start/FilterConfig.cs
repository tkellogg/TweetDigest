using System.Web.Mvc;
using SendGrid.App_Start;

namespace SendGrid
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new LogErrorsAttribute());
        }
    }
}
using System.Web.Mvc;
using TweetDigest.App_Start;

namespace TweetDigest
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
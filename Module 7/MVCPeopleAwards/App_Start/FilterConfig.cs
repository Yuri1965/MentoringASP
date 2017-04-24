using MVCPeopleAwards.Filters;
using System.Web;
using System.Web.Mvc;

namespace MVCPeopleAwards
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());

            filters.Add(new ExceptionFilter());
            filters.Add(new LoggingFilter());
        }
    }
}

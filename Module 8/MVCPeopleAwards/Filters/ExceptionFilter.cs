using MVCPeopleAwards.Helpers;
using MVCPeopleAwards.Models;
using MvcSiteMapProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace MVCPeopleAwards.Filters
{
    public class ExceptionFilter : ActionFilterAttribute, IExceptionFilter
    {
        public void OnException(ExceptionContext filterContext)
        {
            if (!filterContext.ExceptionHandled)
            {
                ErrorViewModel errorModel = ErrorHelper.GetErrorModel(filterContext.Exception.Message, filterContext.Exception.StackTrace, "");

                Logger.LogException(new Exception(String.Format("Ошибка:\n{0}\n{1}\n{2}", 
                    filterContext.Exception.Message,
                    (filterContext.Exception.StackTrace != null) ? filterContext.Exception.StackTrace : "",
                    (filterContext.Exception.InnerException != null) ? filterContext.Exception.InnerException.StackTrace : "")));

                var routeValues = new RouteValueDictionary();
                routeValues.Add("Action", "ShowError");
                routeValues.Add("Controller", "Error");

                filterContext.Controller.TempData["errorModel"] = errorModel;
                filterContext.Result = new RedirectToRouteResult(routeValues);

                filterContext.ExceptionHandled = true;
            }
        }
    }
}
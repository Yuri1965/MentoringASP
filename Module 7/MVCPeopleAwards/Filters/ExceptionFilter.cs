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
                ErrorViewModel errorModel = new ErrorViewModel();
                errorModel.MessageError = filterContext.Exception.Message;
                errorModel.StackTraceError = filterContext.Exception.StackTrace;
                if (filterContext.HttpContext.Request.UrlReferrer != null)
                    errorModel.BackUrl = filterContext.HttpContext.Request.UrlReferrer.AbsoluteUri;
                else
                    errorModel.BackUrl = "";

                Logger.LogException(new Exception(String.Format("Ошибка:\n{0}\n{1}\n{2}", 
                    filterContext.Exception.Message,
                    (filterContext.Exception.StackTrace != null) ? filterContext.Exception.StackTrace : "",
                    (filterContext.Exception.InnerException != null) ? filterContext.Exception.InnerException.StackTrace : "")));

                var routeValues = new RouteValueDictionary();
                routeValues.Add("Action", "ShowError");
                routeValues.Add("Controller", "Error");
                routeValues.Add("errorModel", errorModel);

                SiteMaps.Current.CurrentNode.Title = "Ошибка";

                filterContext.Result = new ViewResult() { ViewName = "Error", ViewData = new ViewDataDictionary(errorModel) };

                //new RedirectToRouteResult("Error", routeValues);

                filterContext.ExceptionHandled = true;
            }
        }
    }
}
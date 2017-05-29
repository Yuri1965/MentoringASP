using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace MVCPeopleAwards.Filters
{
    public class LoggingFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var strB = new StringBuilder();

            strB.AppendLine(String.Format("Поступил запрос: Клиент = {0}", filterContext.HttpContext.Request.UserAgent));
            strB.AppendLine(String.Format("IP адрес: {0}", filterContext.HttpContext.Request.UserHostAddress));
            strB.AppendLine(String.Format("Contoller = {0} Method={1} MethodType = {2}", 
                                filterContext.RouteData.Values["controller"],
                                filterContext.RouteData.Values["action"],
                                filterContext.HttpContext.Request.HttpMethod));

            //вытащим параметры запроса, если они есть (тут по идее только простые писаться будут - если объект = модель то скорее всего не будет)
            //проверил с моделью - не падает в этом случае, НО такого в задании не было!!!!
            var paramExecuting = filterContext.RouteData.Values.Where(x => x.Key != "controller" && x.Key != "action");
            if (paramExecuting != null && paramExecuting.Count() > 0)
            {
                strB.AppendLine("Parameters: ");
                foreach (var p in paramExecuting)
                {
                    strB.AppendLine(String.Format("{0}: {1}", p.Key, p.Value.ToString()));
                }
            }

            Logger.logger.Trace(strB);
        }

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var strB = new StringBuilder();

            strB.AppendLine(String.Format("Запрос обработан: Клиент = {0}", filterContext.HttpContext.Request.UserAgent));
            strB.AppendLine(String.Format("IP адрес: {0}", filterContext.HttpContext.Request.UserHostAddress));
            strB.AppendLine(String.Format("Contoller = {0} Method={1} MethodType = {2}",
                                filterContext.RouteData.Values["controller"],
                                filterContext.RouteData.Values["action"],
                                filterContext.HttpContext.Request.HttpMethod));

            var paramExecuting = filterContext.RouteData.Values.Where(x => x.Key != "controller" && x.Key != "action");
            if (paramExecuting != null && paramExecuting.Count() > 0)
            {
                strB.AppendLine("Parameters: ");
                foreach (var p in paramExecuting)
                {
                    strB.AppendLine(String.Format("{0}: {1}", p.Key, p.Value.ToString()));
                }
            }

            Logger.logger.Trace(strB);
        }
    }
}
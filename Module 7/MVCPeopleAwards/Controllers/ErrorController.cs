using MVCPeopleAwards.Models;
using MvcSiteMapProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVCPeopleAwards.Controllers
{
    public class ErrorController : Controller
    {
        public ActionResult ShowError(int? errorCode)
        {
            ErrorViewModel errorModel = (ErrorViewModel)TempData["errorModel"];

            if (errorModel == null)
            {
                if (errorCode == 404)
                    errorModel = new ErrorViewModel() { MessageError = "Код 404. Указанный ресурс не найден", StackTraceError = "", BackUrl = "" };
                else
                    errorModel = new ErrorViewModel() { MessageError = "Ошибка приложения", StackTraceError = "", BackUrl = "" };
            }

            ViewBag.Title = "Ошибка";
            SiteMaps.Current.CurrentNode.Title = "Ошибка";
            return View("Error", errorModel);
        }
    }
}
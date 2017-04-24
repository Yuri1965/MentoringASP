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
        public ActionResult ShowError()
        {
            ErrorViewModel errorModel = (ErrorViewModel)TempData["errorModel"];

            if (errorModel == null)
                errorModel = new ErrorViewModel() { MessageError = "Неизвестная ошибка", StackTraceError = "", BackUrl = ""};

            ViewBag.Title = "Ошибка";
            SiteMaps.Current.CurrentNode.Title = "Ошибка";
            return View("Error", errorModel);
        }
    }
}
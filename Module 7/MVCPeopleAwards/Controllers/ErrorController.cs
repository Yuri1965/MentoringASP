using MVCPeopleAwards.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVCPeopleAwards.Controllers
{
    public class ErrorController : Controller
    {
        public ActionResult ShowError(ErrorViewModel errorModel)
        {
            ViewBag.Title = "Ошибка";
            return View("Error", errorModel);
        }
    }
}
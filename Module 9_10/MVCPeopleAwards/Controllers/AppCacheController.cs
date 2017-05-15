using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVCPeopleAwards.Controllers
{
    public class AppCacheController : Controller
    {
        // GET: AppCache
        public ActionResult Index()
        {
            return View("Index");
        }
    }
}
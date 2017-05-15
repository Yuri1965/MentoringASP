using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using MVCPeopleAwards.Accounts.DAL.EntityModels;
using MVCPeopleAwards.Accounts.Managers;
using MVCPeopleAwards.Helpers;
using MVCPeopleAwards.Models.Accounts;
using MvcSiteMapProvider;
using MvcSiteMapProvider.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace MVCPeopleAwards.Controllers
{
    [Authorize(Roles = "Admin,CandidateAdmin")]
    public class AdminController : Controller
    {
        private const string DEFAULT_BACK_ERROR_URL = "/admin";
        private AppUserManager UserManager
        {
            get
            {
                return HttpContext.GetOwinContext().GetUserManager<AppUserManager>();
            }
        }

        [Route("admin")]
        public ActionResult Index()
        {
            ViewBag.Title = "Администрирование пользователей";
            return View("Index", UserManager.Users.ToList().Where(x => x.UserName.ToLower().Trim() != "administrator"));
        }

        [Route("account-roles/{id}/edit")]
        public ActionResult EditAccountRoles(string id)
        {
            ApplicationUser user = UserManager.FindByIdAsync(id).Result;
            if (user != null)
            {
                SelectAccountRolesViewModel model = new SelectAccountRolesViewModel(user);

                ViewBag.Title = "Роли пользователя";
                SiteMaps.Current.CurrentNode.Title = ViewBag.Title;

                return View("EditAccountRoles", model);
            }
            else
                return View("Error", ErrorHelper.GetErrorModel("Пользователь не найден", "", DEFAULT_BACK_ERROR_URL));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveAccountRoles(string userId, string[] roleNames)
        {
            ApplicationUser user = UserManager.FindByIdAsync(userId).Result;
            if (user != null)
            {
                ClearUserRoles(user.Id);

                foreach (var role in roleNames)
                {
                    UserManager.AddToRole(userId, role);
                }
            }
            else
                return View("Error", ErrorHelper.GetErrorModel("Пользователь не найден", "", DEFAULT_BACK_ERROR_URL));

            return RedirectToAction("Index");
        }

        private void ClearUserRoles(string userId)
        {
            var roles = UserManager.GetRoles(userId);
            UserManager.RemoveFromRoles(userId, roles.ToArray());
        }

        private void AddErrorsFromResult(IdentityResult result)
        {
            foreach (string error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

    }
}
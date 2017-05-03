using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using MVCPeopleAwards.Accounts.DAL.EntityModels;
using MVCPeopleAwards.Accounts.Managers;
using MVCPeopleAwards.Helpers;
using MVCPeopleAwards.Models.Accounts;
using MvcSiteMapProvider;

namespace MVCPeopleAwards.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private const string DEFAULT_BACK_ERROR_URL = "/peoples";

        private IAuthenticationManager AuthManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private AppUserManager UserManager
        {
            get
            {
                return HttpContext.GetOwinContext().GetUserManager<AppUserManager>();
            }
        }

        [AllowAnonymous]
        [Route("login")]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;

            ViewBag.Title = "Авторизация пользователя";
            SiteMaps.Current.CurrentNode.Title = ViewBag.Title;

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult LoginAuth(LoginViewModel details, string returnUrl)
        {
            ApplicationUser user = UserManager.FindAsync(details.Name, details.Password).Result;

            if (returnUrl == null || returnUrl.Trim() == "")
                returnUrl = DEFAULT_BACK_ERROR_URL;

            if (user == null)
            {
                ModelState.AddModelError("", "Неверное имя пользователя или пароль.");
            }
            else
            {
                ClaimsIdentity ident = UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie).Result;

                AuthManager.SignOut();
                AuthManager.SignIn(
                    new AuthenticationProperties
                    {
                        IsPersistent = false
                    },
                    ident);

                return Redirect(returnUrl);
            }

            ViewBag.ReturnUrl = returnUrl;
            ViewBag.Title = "Авторизация пользователя";
            return View("Login", details);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "PeoplesAward");
        }

        [AllowAnonymous]
        [Route("register-account")]
        public ActionResult Register()
        {
            ViewBag.Title = "Регистрация пользователя";
            SiteMaps.Current.CurrentNode.Title = ViewBag.Title;

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult RegisterSave(CreateAccountViewModel model)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = new ApplicationUser { UserName = model.Name, Email = model.Email };
                IdentityResult result = UserManager.Create(user, model.Password);

                if (result.Succeeded)
                {
                    UserManager.AddToRole(user.Id, "User");

                    ClaimsIdentity ident = UserManager.CreateIdentity(user, DefaultAuthenticationTypes.ApplicationCookie);// CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);

                    AuthManager.SignOut();
                    AuthManager.SignIn(
                        new AuthenticationProperties
                        {
                            IsPersistent = false
                        },
                        ident);

                    return RedirectToAction("Index", "PeoplesAward");
                }
                else
                {
                    AddErrorsFromResult(result);
                }
            }

            ViewBag.Title = "Регистрация пользователя";
            return View("Register", model);
        }

        [Route("edit-currentaccount")]
        public ActionResult Index()
        {
            ApplicationUser user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                EditAccountViewModel userEdit = new EditAccountViewModel()
                {
                    Id = user.Id,
                    Name = user.UserName,
                    Email = user.Email,
                    Password = "",
                    ConfirmPassword = "",
                };

                ViewBag.Title = "Профиль пользователя";
                SiteMaps.Current.CurrentNode.Title = ViewBag.Title;

                return View(userEdit);
            }
            else
            {
                return View("Error", ErrorHelper.GetErrorModel("Пользователь не найден", "", DEFAULT_BACK_ERROR_URL));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditAccountSave(string id, string email, string password, string confirmPassword)
        {
            ApplicationUser user = UserManager.FindById(id);
            if (user != null)
            {
                user.Email = email;
                IdentityResult validEmail = UserManager.UserValidator.ValidateAsync(user).Result;

                if (!validEmail.Succeeded)
                    AddErrorsFromResult(validEmail);

                IdentityResult validPass = null;
                if (password != string.Empty && confirmPassword != string.Empty && password == confirmPassword)
                {
                    validPass = UserManager.PasswordValidator.ValidateAsync(password).Result;

                    if (validPass.Succeeded)
                        user.PasswordHash = UserManager.PasswordHasher.HashPassword(password);
                    else
                        AddErrorsFromResult(validPass);
                }

                if ((validEmail.Succeeded && validPass == null) || (validEmail.Succeeded && password != string.Empty && validPass.Succeeded))
                {
                    IdentityResult result = UserManager.Update(user);
                    if (result.Succeeded)
                        return RedirectToAction("Index", "PeoplesAward");
                    else
                        AddErrorsFromResult(result);
                }

                EditAccountViewModel userEdit = new EditAccountViewModel()
                {
                    Id = user.Id,
                    Name = user.UserName,
                    Email = user.Email,
                    Password = "",
                    ConfirmPassword = "",
                };

                ViewBag.Title = "Профиль пользователя";
                SiteMaps.Current.CurrentNode.Title = ViewBag.Title;

                return View("Index", userEdit);
            }
            else
                return View("Error", ErrorHelper.GetErrorModel("Пользователь не найден", "", DEFAULT_BACK_ERROR_URL));
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
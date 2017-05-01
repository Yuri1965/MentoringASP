using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using MVCPeopleAwards.Accounts.DAL.DBContext;
using MVCPeopleAwards.Accounts.DAL.EntityModels;
using MVCPeopleAwards.Accounts.Managers;
using Owin;
using System;

namespace MVCPeopleAwards.Accounts.Configuration
{
    public class IdentityConfig
    {
        public void Configuration(IAppBuilder app)
        {
            app.CreatePerOwinContext<AppIdentityDbContext>(AppIdentityDbContext.Create);
            app.CreatePerOwinContext<AppUserManager>(AppUserManager.Create);
            //app.CreatePerOwinContext<AppRoleManager>(AppRoleManager.Create);

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login")

                //Provider = new CookieAuthenticationProvider
                //{
                //    // Enables the application to validate the security stamp when the user logs in.
                //    // This is a security feature which is used when you change a password or add an external login to your account.  
                //    OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<AppUserManager, ApplicationUser>(
                //            validateInterval: TimeSpan.FromMinutes(30),
                //            regenerateIdentity: (manager, user) => user.GenerateUserIdentityAsync(manager))
                //}
            });
        }
    }
}
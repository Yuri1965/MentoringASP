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

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),
                // время, в течении которого пользователь считается авторизованным, даже если он ничего не делает
                ExpireTimeSpan = TimeSpan.FromMinutes(180),
                // это если пользователь что-то сделал, то время сбрасывается и по новой считается 180 минут
                SlidingExpiration = true,

                LogoutPath = new PathString("/Account/LogOff")
            });
        }
    }
}
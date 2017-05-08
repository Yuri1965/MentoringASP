using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using MVCPeopleAwards.Accounts.DAL.DBContext;
using MVCPeopleAwards.Accounts.DAL.EntityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCPeopleAwards.Accounts.Managers
{
    public class AppRoleManager : RoleManager<ApplicationRole>, IDisposable
    {
        public AppRoleManager(RoleStore<ApplicationRole> store) : base(store)
        { }

        public static AppRoleManager Create(IdentityFactoryOptions<AppRoleManager> options, IOwinContext context)
        {
            AppIdentityDbContext db = context.Get<AppIdentityDbContext>();

            return new AppRoleManager(new RoleStore<ApplicationRole>(db));
        }
    }
}
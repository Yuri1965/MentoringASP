using Microsoft.AspNet.Identity.EntityFramework;
using MVCPeopleAwards.Accounts.DAL.EntityModels;

namespace MVCPeopleAwards.Accounts.DAL.DBContext
{
    public class AppIdentityDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppIdentityDbContext() : base("MVCPeopleAwards", throwIfV1Schema: false)
        {
        }

        public static AppIdentityDbContext Create()
        {
            return new AppIdentityDbContext();
        }
    }
}
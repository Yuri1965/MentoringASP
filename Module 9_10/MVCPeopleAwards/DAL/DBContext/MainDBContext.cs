using System.Data.Entity;

namespace MVCPeopleAwards.Models.DataDBContext
{
    public class MainDBContext : DbContext
    {
        public DbSet<People> ListPeoples { get; set; }
        public DbSet<Awards> ListAwards { get; set; }
        public DbSet<PeopleAwards> ListPeopleAwards { get; set; }

        public MainDBContext(): base("MVCPeopleAwards")
        {
        }
    }
}
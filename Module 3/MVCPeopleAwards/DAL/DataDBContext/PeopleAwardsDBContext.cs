using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace MVCPeopleAwards.Models.DataDBContext
{
    public class PeopleAwardsDBContext : DbContext
    {
        public DbSet<Peoples> ListPeoples { get; set; }
        public DbSet<Awards> ListAwards { get; set; }
        public DbSet<PeopleAwards> ListPeopleAwards { get; set; }

        public PeopleAwardsDBContext(): base("MVCPeopleAwards")
        {
        }
    }
}
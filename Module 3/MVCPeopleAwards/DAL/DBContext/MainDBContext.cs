using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;

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
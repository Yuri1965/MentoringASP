using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;
using System.Web.Mvc;

namespace MVCPeopleAwards.Models
{
    public class Awards
    {
        public Awards()
        {
            PeopleAwards = new List<PeopleAwards>();
        }

        public int Id { get; set; }

        public string NameAward { get; set; }

        public string DescriptionAward { get; set; }

        public virtual ICollection<PeopleAwards> PeopleAwards { get; set; }
    }
}
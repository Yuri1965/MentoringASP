using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;
using System.Web.Mvc;

namespace MVCPeopleAwards.Models
{
    public class People
    {
        public People()
        {
            PeopleAwards = new List<PeopleAwards>();
        }

        [Key]
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        [Column(TypeName = "date")]
        public DateTime BirthDate { get; set; }

        public virtual ICollection<PeopleAwards> PeopleAwards { get; set; }
    }
}

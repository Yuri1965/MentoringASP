using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;
using System.Web.Mvc;

namespace MVCPeopleAwards.Models
{
    public class Peoples
    {
        public Peoples()
        {
            PeopleAwards = new List<PeopleAwards>();
        }

        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        [Required]
        [Column(TypeName = "date")]
        public DateTime? BirthDate { get; set; }

        public virtual ICollection<PeopleAwards> PeopleAwards { get; set; }
    }
}

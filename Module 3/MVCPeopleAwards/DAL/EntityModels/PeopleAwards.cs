using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace MVCPeopleAwards.Models
{
    public class PeopleAwards
    {
        public int Id { get; set; }

        [Required]
        public int PeopleID { get; set; }

        [Required]
        public int AwardID { get; set; }

        public virtual Awards Award { get; set; }

        public virtual People People { get; set; }
    }
}

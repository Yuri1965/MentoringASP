using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVCPeopleAwards.Models
{
    public class PeopleAwardsModel
    {
        public int Id { get; set; }

        [Required]
        public int PeopleID { get; set; }

        [Required]
        public int AwardID { get; set; }

        [Required]
        public virtual AwardModel Award { get; set; }
    }
}
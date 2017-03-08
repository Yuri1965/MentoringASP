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

        [Required(ErrorMessage = "Поле \"Человек\" должно быть заполнено")]
        public int PeopleID { get; set; }

        [Required(ErrorMessage = "Поле \"Награда\" должно быть заполнено")]
        [Display(Name = "*Выберите награду")]
        public int AwardID { get; set; }

        [Required]
        public virtual AwardModel Award { get; set; }

        private string error = "";
        public string Error { get { return error; } set { error = value; } }

    }
}
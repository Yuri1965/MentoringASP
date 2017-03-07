using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVCPeopleAwards.Models
{
    public class AwardModel
    {
        public AwardModel()
        {
        }

        [HiddenInput(DisplayValue = false)]
        public int Id { get; set; }

        [Required(ErrorMessage = "Это поле должно быть заполнено")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Длина строки должна быть от 3 до 50 символов")]
        [Remote("CheckNameAward", "Awards", AdditionalFields = "Id", HttpMethod = "POST", ErrorMessage = "Такое название награды уже имеется")]
        [Display(Name = "Название награды")]
        public string NameAward { get; set; }

        [Required(ErrorMessage = "Это поле должно быть заполнено")]
        [StringLength(128, MinimumLength = 3, ErrorMessage = "Длина строки должна быть от 3 до 128 символов")]
        [Display(Name = "Описание награды")]
        public string DescriptionAward { get; set; }

        private string error = "";
        public string Error { get { return error; } set { error = value; } }

    }
}
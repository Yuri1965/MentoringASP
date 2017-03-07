using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVCPeopleAwards.Models
{
    public class PeopleModel
    {
        public PeopleModel()
        {
            PeopleAwards = new List<PeopleAwardsModel>();
        }

        [HiddenInput(DisplayValue = false)]
        public int Id { get; set; }

        [Required(ErrorMessage = "Это поле должно быть заполнено")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Длина строки должна быть от 3 до 50 символов")]
        [Display(Name = "Фамилия")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Это поле должно быть заполнено")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Длина строки должна быть от 3 до 50 символов")]
        [Display(Name = "Имя")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Это поле должно быть заполнено")]
        [Display(Name = "Дата рождения")]
        [Column(TypeName = "date")]
        public DateTime? BirthDate { get; set; }

        [Display(Name = "Возраст (лет)")]
        public int Age
        {
            get
            {
                return GetAge();
            }
        }

        public List<PeopleAwardsModel> PeopleAwards { get; set; }

        private int GetAge()
        {
            if (!this.BirthDate.HasValue)
                return 0;

            DateTime birthDate = this.BirthDate.GetValueOrDefault();

            int years = DateTime.Now.Year - birthDate.Year;
            if (DateTime.Now.Month < birthDate.Month || (DateTime.Now.Month == birthDate.Month && DateTime.Now.Day < birthDate.Day))
                years--;
            return years;
        }
    }
}
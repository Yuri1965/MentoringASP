﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web;
using System.Web.Mvc;

namespace MVCPeopleAwards.Models
{
    public class PeopleViewModel
    {
        public PeopleViewModel()
        {
            PeopleAwards = new List<ListPeopleAwardsViewModel>();
        }

        [HiddenInput(DisplayValue = false)]
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Это поле должно быть заполнено")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Длина строки должна быть от 1 до 50 символов")]
        [Display(Name = "*Фамилия")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Это поле должно быть заполнено")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Длина строки должна быть от 1 до 50 символов")]
        [Display(Name = "*Имя")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Это поле должно быть заполнено")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd.mm.yy}", ApplyFormatInEditMode = true)]
        [Display(Name = "*Дата рождения")]
        public DateTime BirthDate { get; set; }

        public string BirthDateStr
        {
            get
            {
                return GetBirthDateStr();
            }
        }

        [Display(Name = "Возраст (лет)")]
        public int Age
        {
            get
            {
                return GetAge();
            }
        }

        public List<ListPeopleAwardsViewModel> PeopleAwards { get; set; }

        [Display(Name = "Выберите награду")]
        [Remote("CheckPeopleAward", "PeoplesAward", AdditionalFields = "peopleId", HttpMethod = "POST", ErrorMessage = "Такая награда уже имеется")]
        public int SelectedAwardID { get; set; }

        public IEnumerable<SelectListItem> Awards { get; set; }

        [Display(Name = "Фото")]
        public HttpPostedFileBase PhotoPeople { get; set; }

        public string PhotoMIMEType { get; set; }

        public bool ImageIsEmpty { get; set; }

        private string error = "";
        public string Error { get { return error; } set { error = value; } }

        private string GetBirthDateStr()
        {
            return BirthDate.ToString("dd.MM.yyyy");
        }

        private int GetAge()
        {
            int years = DateTime.Now.Year - this.BirthDate.Year;
            if (DateTime.Now.Month < this.BirthDate.Month || (DateTime.Now.Month == this.BirthDate.Month && DateTime.Now.Day < this.BirthDate.Day))
                years--;
            return years;
        }
    }
}
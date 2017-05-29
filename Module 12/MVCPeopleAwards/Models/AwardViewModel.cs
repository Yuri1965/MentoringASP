using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;

namespace MVCPeopleAwards.Models
{
    public class AwardViewModel
    {
        public AwardViewModel()
        {
        }

        [HiddenInput(DisplayValue = false)]
        public int Id { get; set; }

        [Required(ErrorMessage = "Это поле должно быть заполнено")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Длина строки должна быть от 3 до 50 символов")]
        //[Remote("CheckNameAward", "Awards", AdditionalFields = "Id", HttpMethod = "GET", ErrorMessage = "Такое название награды уже имеется")]
        [RegularExpression("^([a-zA-Z0-9 -]+)$", ErrorMessage = "Наименование может содержать Латинские буквы, Цифры, Пробел или знак Дефиса")]
        [Display(Name = "*Название награды")]
        public string NameAward { get; set; }

        [StringLength(250, ErrorMessage = "Длина строки должна быть не более 250 символов")]
        [Display(Name = "Описание награды")]
        public string DescriptionAward { get; set; }

        [Display(Name = "Фото награды")]
        public byte[] PhotoAward { get; set; }

        public string PhotoMIMEType { get; set; }

        public bool ImageIsEmpty { get; set; }

        private string error = "";
        public string Error { get { return error; } set { error = value; } }
    }
}
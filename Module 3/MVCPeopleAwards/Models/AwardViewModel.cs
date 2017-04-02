using System.ComponentModel.DataAnnotations;
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
        [Remote("CheckNameAward", "Awards", AdditionalFields = "Id", HttpMethod = "POST", ErrorMessage = "Такое название награды уже имеется")]
        [Display(Name = "*Название награды")]
        public string NameAward { get; set; }

        [Required(ErrorMessage = "Это поле должно быть заполнено")]
        [StringLength(128, MinimumLength = 3, ErrorMessage = "Длина строки должна быть от 3 до 128 символов")]
        [Display(Name = "*Описание награды")]
        public string DescriptionAward { get; set; }

        [Display(Name = "Фото награды")]
        public byte[] PhotoAward { get; set; }

        public string PhotoMIMEType { get; set; }

        public bool ImageIsEmpty { get; set; }

        private string error = "";
        public string Error { get { return error; } set { error = value; } }
    }
}
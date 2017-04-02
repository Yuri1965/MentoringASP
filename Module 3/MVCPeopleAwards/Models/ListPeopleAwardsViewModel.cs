using System.ComponentModel.DataAnnotations;

namespace MVCPeopleAwards.Models
{
    public class ListPeopleAwardsViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Поле \"Человек\" должно быть заполнено")]
        public int PeopleID { get; set; }

        [Required(ErrorMessage = "Поле \"Награда\" должно быть заполнено")]
        [Display(Name = "*Выберите награду")]
        public int AwardID { get; set; }

        [Required]
        public AwardViewModel Award { get; set; }

        private string error = "";
        public string Error { get { return error; } set { error = value; } }

    }
}
using System.Collections.Generic;

namespace MVCPeopleAwards.Models
{
    public class ListAwardsViewModel
    {
        public ListAwardsViewModel()
        {
            error = "";
            ListAwards = new List<AwardViewModel>();
            AwardModel = new AwardViewModel()
            {
                Id = 0,
                NameAward = "",
                DescriptionAward = "",
                PhotoAward = null,
                ImageIsEmpty = true,
                PhotoMIMEType = ""
            };
        }

        private string error = "";
        public string Error { get { return error; } set { error = value; } }

        public AwardViewModel AwardModel { get; set; }

        public List<AwardViewModel> ListAwards { get; set; }
    }
}
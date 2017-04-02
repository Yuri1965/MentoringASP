using System.Collections.Generic;

namespace MVCPeopleAwards.Models
{
    public class ListAwardsViewModel
    {
        private string error = "";
        public string Error { get { return error; } set { error = value; } }

        public List<AwardViewModel> ListAwards { get; set; }
    }
}
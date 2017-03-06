using System.Collections.Generic;

namespace MVCPeopleAwards.Models
{
    public class AwardsModelView
    {
        private string error = "";
        public string Error { get { return error; } set { error = value; } }

        public List<AwardModel> ListAwards { get; set; }

        public string GetDisplayFieldName(string fieldName)
        {
            if (fieldName.Equals("NameAward"))
                return "Название награды";
            if (fieldName.Equals("DescriptionAward"))
                return "Описание награды";

            return "";
        }
    }
}
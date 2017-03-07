using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCPeopleAwards.Models
{
    public class PeopleModelView
    {
        private string error = "";
        public string Error { get { return error; } set { error = value; } }

        public List<PeopleModel> ListPeople { get; set; }

        public string GetDisplayFieldName(string fieldName)
        {
            if (fieldName.Equals("FirstName"))
                return "Имя";
            if (fieldName.Equals("LastName"))
                return "Фамилия";
            if (fieldName.Equals("BirthDate"))
                return "Дата рождения";
            if (fieldName.Equals("Age"))
                return "Возраст (лет)";

            return "";
        }
    }
}
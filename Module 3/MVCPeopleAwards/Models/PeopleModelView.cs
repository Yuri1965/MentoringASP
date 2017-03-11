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
    }
}
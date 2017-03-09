using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCPeopleAwards.Repositories
{
    public static class UtilHelper
    {
        public static bool CheckBirthDate(DateTime checkDate)
        {
            int years = DateTime.Now.Year - checkDate.Year;
            if (DateTime.Now.Month < checkDate.Month || (DateTime.Now.Month == checkDate.Month && DateTime.Now.Day < checkDate.Day))
                years--;

            if (years < 5 || years > 120)
                return false;
            else
                return true;
        }


    }
}
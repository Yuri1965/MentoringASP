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

            if (years < 5 || years > 150)
                return false;
            else
                return true;
        }

        public static Byte[] HttpPostedFileBaseToByte(HttpPostedFileBase file)
        {
            if (file == null || file.ContentLength <= 0)
                return null;

            var array = new Byte[file.ContentLength];

            file.InputStream.Position = 0;
            file.InputStream.Read(array, 0, file.ContentLength);
            return array;
        }
    }
}
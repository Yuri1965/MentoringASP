using MVCPeopleAwards.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;

namespace MVCPeopleAwards.Helpers
{
    public static class ErrorHelper
    {
        public static ErrorViewModel GetErrorModel(string messageError, string stackTraceError = "", string backUrl = "")
        {
            ErrorViewModel errorModel = new ErrorViewModel();
            {
                errorModel.MessageError = messageError;
                errorModel.StackTraceError = stackTraceError;
                errorModel.BackUrl = backUrl;
            }
            return errorModel;
        }

        internal static string GetErrorModel(string v, StringBuilder strb, string dEFAULT_BACK_ERROR_URL)
        {
            throw new NotImplementedException();
        }
    }
}